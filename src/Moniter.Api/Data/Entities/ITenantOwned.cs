namespace Moniter.Api.Data.Entities;

public interface ITenantOwned
{
    Guid OrganizationId { get; set; }
}
