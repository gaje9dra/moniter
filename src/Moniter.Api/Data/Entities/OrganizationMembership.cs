namespace Moniter.Api.Data.Entities;

public enum MembershipRole
{
    Owner,
    Administrator,
    Manager,
    Analyst,
    Auditor,
    Member
}

public enum MembershipStatus
{
    Invited,
    Active,
    Suspended
}

public sealed class OrganizationMembership : ITenantOwned, ITimestampedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid UserIdentityId { get; set; }

    public MembershipRole Role { get; set; } = MembershipRole.Member;

    public MembershipStatus Status { get; set; } = MembershipStatus.Invited;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Organization Organization { get; set; } = null!;

    public UserIdentity UserIdentity { get; set; } = null!;
}
