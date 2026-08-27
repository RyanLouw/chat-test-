using System.Net;
using System.Text.Json;
using ILogger = Serilog.ILogger;

namespace CallQuality.Middleware;

public sealed class ControllerExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ControllerExceptionMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unhandled exception occurred and was caught by ControllerExceptionMiddleware. {ExceptionMessage}", ex.Message);

            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = ex.Message
            };

            context.Response.StatusCode = ex switch
            {
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

}