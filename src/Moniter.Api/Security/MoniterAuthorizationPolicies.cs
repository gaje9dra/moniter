using Microsoft.AspNetCore.Authorization;

namespace Moniter.Api.Security;

public static class MoniterAuthorizationPolicies
{
    public const string OrganizationMember = "moniter:organization-member";
    public const string OrganizationManager = "moniter:organization-manager";
    public const string OrganizationAdministrator = "moniter:organization-administrator";
    public const string OrganizationAdministratorWithMfa = "moniter:organization-administrator-mfa";

    public static void Configure(AuthorizationOptions options)
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        options.AddPolicy(OrganizationMember, policy => policy
            .RequireAuthenticatedUser()
            .RequireClaim(MoniterClaimTypes.OrganizationContextResolved, "true")
            .RequireClaim(MoniterClaimTypes.OrganizationId)
            .RequireClaim(MoniterClaimTypes.UserIdentityId));

        options.AddPolicy(OrganizationManager, policy => policy
            .RequireAuthenticatedUser()
            .RequireClaim(MoniterClaimTypes.OrganizationContextResolved, "true")
            .RequireClaim(MoniterClaimTypes.OrganizationId)
            .RequireClaim(MoniterClaimTypes.OrganizationRole, "Owner", "Administrator", "Manager"));

        options.AddPolicy(OrganizationAdministrator, policy => policy
            .RequireAuthenticatedUser()
            .RequireClaim(MoniterClaimTypes.OrganizationContextResolved, "true")
            .RequireClaim(MoniterClaimTypes.OrganizationId)
            .RequireClaim(MoniterClaimTypes.OrganizationRole, "Owner", "Administrator"));

        options.AddPolicy(OrganizationAdministratorWithMfa, policy => policy
            .RequireAuthenticatedUser()
            .RequireClaim(MoniterClaimTypes.OrganizationContextResolved, "true")
            .RequireClaim(MoniterClaimTypes.OrganizationId)
            .RequireClaim(MoniterClaimTypes.OrganizationRole, "Owner", "Administrator")
            .RequireClaim(MoniterClaimTypes.MfaAssurance, "true"));
    }
}
