using System.Security.Claims;
using Moniter.Api.Data.Tenancy;

namespace Moniter.Api.Security;

public static class MoniterClaimTypes
{
    public const string TrustedIssuer = "urn:moniter:trusted_issuer";
    public const string UserIdentityId = "urn:moniter:user_identity_id";
    public const string OrganizationId = TenantClaimTypes.OrganizationId;
    public const string OrganizationRole = "urn:moniter:organization_role";
    public const string OrganizationContextResolved = "urn:moniter:organization_context_resolved";
    public const string MfaAssurance = "urn:moniter:mfa_assurance";

    public static bool IsServerContextClaim(string claimType) =>
        claimType is OrganizationId
            or UserIdentityId
            or OrganizationRole
            or OrganizationContextResolved
            or MfaAssurance;

    public static bool IsServerManagedClaim(string claimType) =>
        claimType == TrustedIssuer || IsServerContextClaim(claimType);

    public static void RemoveServerContextClaims(ClaimsPrincipal principal)
    {
        foreach (var identity in principal.Identities)
        {
            foreach (var claim in identity.Claims.Where(claim => IsServerContextClaim(claim.Type)).ToArray())
            {
                identity.RemoveClaim(claim);
            }
        }
    }

    public static void RemoveServerManagedClaims(ClaimsPrincipal principal)
    {
        foreach (var identity in principal.Identities)
        {
            foreach (var claim in identity.Claims.Where(claim => IsServerManagedClaim(claim.Type)).ToArray())
            {
                identity.RemoveClaim(claim);
            }
        }
    }
}
