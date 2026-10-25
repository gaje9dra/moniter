using Microsoft.Extensions.Configuration;

namespace Moniter.Api.Data;

public static class DatabaseConnectionString
{
    public static string? TryResolve(IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("Moniter");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        var environmentConnectionString = configuration["MONITER_DATABASE_URL"];
        return string.IsNullOrWhiteSpace(environmentConnectionString)
            ? null
            : environmentConnectionString;
    }
}
