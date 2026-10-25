using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moniter.Api.Security;

namespace Moniter.Api.Data.Tenancy;

public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public Guid? OrganizationId
    {
        get
        {
            ClaimsPrincipal? principal = httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var resolvedClaims = principal.FindAll(MoniterClaimTypes.OrganizationContextResolved).ToArray();
            var organizationClaims = principal.FindAll(TenantClaimTypes.OrganizationId).ToArray();
            if (resolvedClaims.Length != 1 ||
                resolvedClaims[0].Value != "true" ||
                organizationClaims.Length != 1 ||
                !Guid.TryParse(organizationClaims[0].Value, out var organizationId) ||
                organizationId == Guid.Empty)
            {
                return null;
            }

            return organizationId;
        }
    }
}
