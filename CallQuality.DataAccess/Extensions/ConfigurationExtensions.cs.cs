using Microsoft.Extensions.Configuration;

namespace CallQuality.Core.Extensions;

public static class ConfigurationExtensions
{
    public static Guid GetRequiredGuid(this IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (!Guid.TryParse(value, out var guid))
        {
            throw new InvalidOperationException(
                $"Missing or invalid configuration value for '{key}'. Value was '{value ?? "null"}'.");
        }

        return guid;
    }
}