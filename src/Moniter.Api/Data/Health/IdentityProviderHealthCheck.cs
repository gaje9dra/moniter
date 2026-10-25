using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moniter.Api.Security;

namespace Moniter.Api.Data.Health;

public sealed class IdentityProviderHealthCheck(
    IdentityProviderConfiguration configuration,
    IHostEnvironment environment) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var problem = configuration.GetReadinessProblem(environment);
        var result = problem is null
            ? HealthCheckResult.Healthy("Identity-provider and MFA configuration is present.")
            : HealthCheckResult.Unhealthy("Identity-provider authentication is not fully configured.");

        return Task.FromResult(result);
    }
}
