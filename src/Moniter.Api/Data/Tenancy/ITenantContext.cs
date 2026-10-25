namespace Moniter.Api.Data.Tenancy;

/// <summary>
/// Exposes the organization resolved for the current request from a trusted,
/// authenticated server-side principal. It must never be populated directly
/// from a request header, route value, or query parameter.
/// </summary>
public interface ITenantContext
{
    Guid? OrganizationId { get; }
}

public static class TenantContextExtensions
{
    public static Guid GetRequiredOrganizationId(this ITenantContext tenantContext)
    {
        if (tenantContext.OrganizationId is not { } organizationId || organizationId == Guid.Empty)
        {
            throw new TenantScopeRequiredException();
        }

        return organizationId;
    }
}

public sealed class TenantScopeRequiredException : InvalidOperationException
{
    public TenantScopeRequiredException()
        : base("A trusted organization context is required for tenant-owned data access.")
    {
    }
}

public static class TenantClaimTypes
{
    public const string OrganizationId = "urn:moniter:organization_id";
}
