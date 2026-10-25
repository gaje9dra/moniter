using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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

            var claims = principal.FindAll(TenantClaimTypes.OrganizationId).ToArray();
            if (claims.Length != 1 ||
                !Guid.TryParse(claims[0].Value, out var organizationId) ||
                organizationId == Guid.Empty)
            {
                return null;
            }

            return organizationId;
        }
    }
}
