using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Moniter.Api.Security;

public sealed class SecurityAuditAuthorizationResultHandler(
    ILogger<SecurityAuditAuthorizationResultHandler> logger) : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded)
        {
            logger.LogWarning(
                "security_audit event=authorization_denied outcome={Outcome} path={Path} correlation_id={CorrelationId}",
                authorizeResult.Challenged ? "challenge" : "forbid",
                context.Request.Path.Value,
                context.TraceIdentifier);
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
