namespace Moniter.Api.Data.Entities;

/// <summary>
/// Represents an identity established by a future trusted identity provider.
/// Moniter stores no password or authentication secret in this table.
/// </summary>
public sealed class UserIdentity : ITimestampedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Issuer { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? DisplayName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<OrganizationMembership> Memberships { get; set; } = new List<OrganizationMembership>();
}
