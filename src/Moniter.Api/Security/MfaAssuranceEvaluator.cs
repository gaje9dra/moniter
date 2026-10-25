using System.Security.Claims;

namespace Moniter.Api.Security;

public static class MfaAssuranceEvaluator
{
    public static bool IsSatisfied(
        ClaimsPrincipal principal,
        IdentityProviderConfiguration configuration)
    {
        var hasAcceptedAuthenticationMethod = principal.FindAll("amr")
            .Any(claim => configuration.MfaAmrValues.Contains(claim.Value));
        var hasAcceptedAuthenticationContext = principal.FindAll("acr")
            .Any(claim => configuration.MfaAcrValues.Contains(claim.Value));

        return hasAcceptedAuthenticationMethod || hasAcceptedAuthenticationContext;
    }
}
