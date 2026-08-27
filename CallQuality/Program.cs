using CallQuality.Core.DataAccess.ADUsersDataAccess;
using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.DataAccess.Context;
using CallQuality.Core.DataAccess.DischemPRPDataAccess;
using CallQuality.Core.DataAccess.DischemSRSDataAccess;
using CallQuality.Core.DataAccess.PRPDataAccess;
using CallQuality.Core.DataAccess.PSPAbbvieDataAccess;
using CallQuality.Core.DataAccess.PSPDataAccess;
using CallQuality.Core.DataAccess.PSPDataAccess.Models;
using CallQuality.Core.DataAccess.ThreeCXDataAccess;
using CallQuality.Core.DataAccess.TrainingRegisterDataAccess;
using CallQuality.Core.DataAccess.TrainingRegisterDataAccess.Models;
using CallQuality.Core.Helpers;
using CallQuality.Core.Manager.AssessmentsManager;
using CallQuality.Core.Manager.ExportManager;
using CallQuality.Core.Manager.OperatorAssignmentManager;
using CallQuality.Core.Manager.QuestionsManager;
using CallQuality.Core.Manager.ReportManager;
using CallQuality.Core.Manager.TrainingManager;
using CallQuality.Core.Resources;
using CallQuality.DataAccess.DataAccess.PSPDataAccess;
using CallQuality.DataAccess.DataAccess.ThreeCXDataAccess;
using CallQuality.DataAccess.DataAccess.TrainingRegisterDataAccess;
using CallQuality.HealthChecks;
using CallQuality.Middleware;
using CallQuality.Middleware.Interfaces;
using CallQuality.Services;
using CallQuality.Utilities;
using HW.CentralConfig.Package.Core;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
await builder.AddCentralConfigAsync();

try
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

    builder.Host.UseSerilog();

    builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

    builder.Services
        .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(
            builder.Configuration.GetSection("AzureAd"));


    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        options.AddPolicy(
            "Manager",
            policy => policy.Requirements.Add(
                new ManagerRequirement()));

        options.AddPolicy(
            "Assessor",
            policy => policy.Requirements.Add(
                new AssessorRequirement()));

        options.AddPolicy(
            "It",
            policy => policy.Requirements.Add(
                new ItRequirement()));

        options.AddPolicy(
            "AssessorOrManager",
            policy => policy.Requirements.Add(
                new AssessorOrManagerRequirement()));
    });

    builder.Services.AddScoped<IAuthorizationHandler, ManagerHandler>();
    builder.Services.AddScoped<IAuthorizationHandler, AssessorHandler>();
    builder.Services.AddScoped<IAuthorizationHandler, ItHandler>();
    builder.Services.AddScoped<
        IAuthorizationHandler,
        AssessorOrManagerHandler>();

    /*
     * MVC and HTTP context
     */
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpContextAccessor();

    /*
     * Session
     */
    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(1);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    /*
     * Health checks
     */
    builder.Services
        .AddHealthChecks()
        .AddCheck<ApplicationHealthCheck>("application");

    /*
     * Application services and managers
     */
    builder.Services.AddScoped<IUserSession, UserSession>();

    builder.Services.AddScoped<IExportService, ExportManager>();
    builder.Services.AddScoped<IReportManager, ReportManager>();
    builder.Services.AddScoped<IQuestionsManager, QuestionsManager>();
    builder.Services.AddScoped<
        ICallAssessmentManager,
        CallAssessmentManager>();
    builder.Services.AddScoped<ITrainingManager, TrainingManager>();
    builder.Services.AddScoped<IOperatorManager, OperatorManager>();

    /*
     * Database contexts and data access
     */


    builder.Services.AddDbContext<CallQualityDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "CallQualityDb")));

    builder.Services.AddScoped<
        ICallQualityDataAccess,
        CallQualityDataAccess>();

    builder.Services.AddDbContext<DischemPRPDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DischemPRP")));

    builder.Services.AddScoped<
        IDischemPRPDataAccess,
        DischemPRPDataAccess>();

    builder.Services.AddDbContext<DischemSRSDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DischemSRS")));

    builder.Services.AddScoped<
        IDischemSRSDataAccess,
        DischemSRSDataAccess>();

    builder.Services.AddDbContext<PRPDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "PRP")));

    builder.Services.AddScoped<IPRPDataAccess, PRPDataAccess>();

    builder.Services.AddDbContext<ADUsersDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "ADUser")));

    builder.Services.AddScoped<
        IADUsersDataAccess,
        ADUsersDataAccess>();

    /*
     * Training Register API
     */
    builder.Services.Configure<TrainingRegisterApi>(
        builder.Configuration.GetSection(
            "ApiAccess:TrainingRegister"));

    builder.Services
        .AddHttpClient<
            ITrainingRegisterDataAccess,
            TrainingRegisterDataAccess>(client =>
            {
                var baseUrl =
                    builder.Configuration[
                        "ApiAccess:TrainingRegister:BaseUrl"];

                var apiKey =
                    builder.Configuration[
                        "ApiAccess:TrainingRegister:Key"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    throw new InvalidOperationException(
                        "Missing ApiAccess:TrainingRegister:BaseUrl");
                }

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException(
                        "Missing ApiAccess:TrainingRegister:Key");
                }

                client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Add(
                    "x-api-key",
                    apiKey);

                client.DefaultRequestHeaders.Add(
                    "ApiKey",
                    apiKey);
            });



    builder.Services.Configure<EmailSettings>(
        builder.Configuration.GetSection("Email"));

    builder.Services.AddScoped<EmailHelper>();

    /*
     * PSP named options
     */
    builder.Services.Configure<PSPApi>(
        "NormalPSP",
        builder.Configuration.GetSection(
            "ApiAccess:PSP"));

    builder.Services.Configure<PSPApi>(
        "AbbViePSP",
        builder.Configuration.GetSection(
            "ApiAccess:AbbViePSPApi"));

    /*
     * PSP HTTP clients
     */
    builder.Services
        .AddHttpClient<IPSPDataAccess, PSPDataAccess>();
    //.AddHttpMessageHandler<
    //    CentralConfigPackageTokenMiddleware>();

    builder.Services
        .AddHttpClient<IPSPAbbvieDataAccess, PSPAbbvieDataAccess>();
        //.AddHttpMessageHandler<
        //    CentralConfigPackageTokenMiddleware>();

    /*
     * Token provider required by CallQualityBearerTokenHandler
     */
    builder.Services
        .AddHttpClient<IAuthTokenProvide, AuthTokenProvide>(client =>
        {
            var baseUrl =
                builder.Configuration["CentralConfig:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException(
                    "Missing CentralConfig:BaseUrl");
            }

            client.BaseAddress = new Uri(baseUrl);
        });

    builder.Services.AddTransient<
        CallQualityBearerTokenHandler>();


    builder.Services
        .AddHttpClient<IThreeCXDataAccess, ThreeCXDataAccess>(
            client =>
            {
                var baseUrl =
                    builder.Configuration[
                        "ApiAccess:ThreeCX:BaseUrl"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    throw new InvalidOperationException(
                        "Missing ApiAccess:ThreeCX:BaseUrl");
                }

                client.BaseAddress = new Uri(baseUrl);
            })
        .AddHttpMessageHandler<
            CallQualityBearerTokenHandler>();

    var app = builder.Build();


    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Nav/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseMiddleware<ControllerExceptionMiddleware>();

    app.UseSession();

    app.UseAuthentication();
    app.UseAuthorization();

    /*
     * Endpoints
     */
    app.MapHealthChecks(
        "/_health",
        new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType =
                    "application/json";

                var response = new
                {
                    status = report.Status.ToString(),

                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        data = entry.Value.Data
                    }),

                    totalDuration =
                        report.TotalDuration.ToString()
                };

                await JsonSerializer.SerializeAsync(
                    context.Response.Body,
                    response,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
            }
        })
        .AllowAnonymous();

    app.MapControllerRoute(
        name: "default",
        pattern:
            "{controller=Nav}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(
        ex,
        "Application failed startup");
}
finally
{
    await Log.CloseAndFlushAsync();
}