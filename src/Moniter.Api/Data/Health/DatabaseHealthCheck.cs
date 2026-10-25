using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moniter.Api.Data;
using Npgsql;

namespace Moniter.Api.Data.Health;

public sealed class DatabaseHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var connectionString = DatabaseConnectionString.TryResolve(configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Unhealthy("PostgreSQL connection is not configured.");
        }

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new NpgsqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy("PostgreSQL is reachable.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            // Do not expose connection strings, provider messages, hosts, or other
            // deployment details through unauthenticated health endpoints.
            return HealthCheckResult.Unhealthy("PostgreSQL connectivity check failed.");
        }
    }
}
