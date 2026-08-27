using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Reflection;

namespace CallQuality.HealthChecks;

public sealed class ApplicationHealthCheck : IHealthCheck
{
    private static readonly FileVersionInfo? VersionInfo =
        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

    private readonly IConfiguration _configuration;

    public ApplicationHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var connectionChecks = new Dictionary<string, bool>
        {
            ["CallQualityDb"] = IsConnectionStringResolved("CallQualityDb"),
            ["DischemPRP"] = IsConnectionStringResolved("DischemPRP"),
            ["DischemSRS"] = IsConnectionStringResolved("DischemSRS"),
            ["PRP"] = IsConnectionStringResolved("PRP"),
            ["ADUser"] = IsConnectionStringResolved("ADUser")
        };

        var allConnectionStringsResolved = connectionChecks.All(x => x.Value);

        var data = new Dictionary<string, object>
        {
            ["Version"] = VersionInfo?.FileVersion ?? "Unknown",
            ["Name"] = VersionInfo?.FileDescription ?? "CallQuality",
            ["ConnectionStringsResolved"] = connectionChecks
        };

        return Task.FromResult(
            allConnectionStringsResolved
                ? HealthCheckResult.Healthy("Application is healthy.", data)
                : HealthCheckResult.Unhealthy("One or more connection strings are missing or unresolved.", data: data));
    }

    private bool IsConnectionStringResolved(string name)
    {
        var value = _configuration.GetConnectionString(name);

        return !string.IsNullOrWhiteSpace(value)
               && !value.Contains('#')
               && !value.Contains('{')
               && !value.Contains('}');
    }
}