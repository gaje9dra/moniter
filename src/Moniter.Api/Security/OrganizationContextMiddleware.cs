using System.Security.Claims;
using System.Text.Json;
using Moniter.Api.Data.Entities;

namespace Moniter.Api.Security;

public sealed class OrganizationContextMiddleware(
    RequestDelegate next,
    IdentityProviderConfiguration identityProvider,
    ILogger<OrganizationContextMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api") ||
            context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        // Resolve scoped database services only when an authenticated API request
        // actually needs organization lookup. Health and anonymous routes must not
        // instantiate the database context.
        var resolver = context.RequestServices.GetRequiredService<IOrganizationContextResolver>();

        MoniterClaimTypes.RemoveServerContextClaims(context.User);

        Guid? selectedOrganizationId = null;
        if (context.Request.Headers.TryGetValue("X-Organization-Id", out var organizationHeader))
        {
            if (organizationHeader.Count != 1 ||
                !Guid.TryParse(organizationHeader[0], out var parsedOrganizationId) ||
                parsedOrganizationId == Guid.Empty)
            {
                await WriteProblemAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    "The organization selector is invalid.",
                    context.RequestAborted);
                return;
            }

            selectedOrganizationId = parsedOrganizationId;
        }

        var issuerClaims = context.User.FindAll(MoniterClaimTypes.TrustedIssuer)
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        var subjectClaims = context.User.FindAll("sub")
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        if (issuerClaims.Length != 1 ||
            subjectClaims.Length != 1 ||
            issuerClaims[0].Length > 255 ||
            subjectClaims[0].Length > 255)
        {
            logger.LogWarning(
                "security_audit event=organization_context_denied reason=missing_trusted_identity correlation_id={CorrelationId}",
                context.TraceIdentifier);
            await WriteProblemAsync(
                context,
                StatusCodes.Status403Forbidden,
                "The authenticated identity is not authorized for an organization.",
                context.RequestAborted);
            return;
        }

        var resolution = await resolver.ResolveAsync(
            issuerClaims[0],
            subjectClaims[0],
            selectedOrganizationId,
            context.RequestAborted);

        if (!resolution.Succeeded ||
            resolution.UserIdentityId is not { } userIdentityId ||
            resolution.OrganizationId is not { } organizationId ||
            resolution.Role is not { } role)
        {
            var selectionRequired = resolution.Failure == OrganizationContextFailure.OrganizationSelectionRequired;
            logger.LogWarning(
                "security_audit event=organization_context_denied reason={Reason} correlation_id={CorrelationId}",
                resolution.Failure,
                context.TraceIdentifier);
            await WriteProblemAsync(
                context,
                selectionRequired ? StatusCodes.Status400BadRequest : StatusCodes.Status403Forbidden,
                selectionRequired
                    ? "Choose an organization you belong to using X-Organization-Id."
                    : "The authenticated identity is not authorized for the selected organization.",
                context.RequestAborted);
            return;
        }

        var mfaSatisfied = MfaAssuranceEvaluator.IsSatisfied(context.User, identityProvider);
        if (RequiresMfa(role) && !mfaSatisfied)
        {
            logger.LogWarning(
                "security_audit event=privileged_mfa_denied user_identity_id={UserIdentityId} organization_id={OrganizationId} role={Role} correlation_id={CorrelationId}",
                userIdentityId,
                organizationId,
                role,
                context.TraceIdentifier);
            await WriteProblemAsync(
                context,
                StatusCodes.Status403Forbidden,
                "This organization role requires a verified multi-factor authentication assertion.",
                context.RequestAborted);
            return;
        }

        var serverContext = new ClaimsIdentity("MoniterResolvedOrganization");
        serverContext.AddClaim(new Claim(MoniterClaimTypes.UserIdentityId, userIdentityId.ToString("D")));
        serverContext.AddClaim(new Claim(MoniterClaimTypes.OrganizationId, organizationId.ToString("D")));
        serverContext.AddClaim(new Claim(MoniterClaimTypes.OrganizationRole, role.ToString()));
        serverContext.AddClaim(new Claim(MoniterClaimTypes.OrganizationContextResolved, "true"));
        if (mfaSatisfied)
        {
            serverContext.AddClaim(new Claim(MoniterClaimTypes.MfaAssurance, "true"));
        }

        context.User.AddIdentity(serverContext);
        logger.LogInformation(
            "security_audit event=organization_context_resolved user_identity_id={UserIdentityId} organization_id={OrganizationId} role={Role} correlation_id={CorrelationId}",
            userIdentityId,
            organizationId,
            role,
            context.TraceIdentifier);

        await next(context);
    }

    private static bool RequiresMfa(MembershipRole role) =>
        role is MembershipRole.Owner or MembershipRole.Administrator or MembershipRole.Manager;

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        CancellationToken cancellationToken)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            new
            {
                type = $"https://httpstatuses.com/{statusCode}",
                title,
                status = statusCode,
                correlationId = context.TraceIdentifier
            },
            cancellationToken: cancellationToken);
    }
}
