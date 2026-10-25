namespace Moniter.Api.Data.Entities;

public sealed class Department : ITenantOwned, ITimestampedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid? ParentDepartmentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Organization Organization { get; set; } = null!;

    public Department? ParentDepartment { get; set; }

    public ICollection<Department> Children { get; set; } = new List<Department>();
}
