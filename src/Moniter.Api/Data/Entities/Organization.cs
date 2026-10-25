namespace Moniter.Api.Data.Entities;

public enum OrganizationStatus
{
    Active,
    Suspended
}

public sealed class Organization : ITimestampedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<OrganizationMembership> Memberships { get; set; } = new List<OrganizationMembership>();

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
