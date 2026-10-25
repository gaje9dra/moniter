using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data;
using Moniter.Api.Data.Entities;

namespace Moniter.Api.Security;

public enum OrganizationContextFailure
{
    None,
    IdentityNotProvisioned,
    NoActiveMembership,
    OrganizationSelectionRequired
}

public sealed record OrganizationContextResolution(
    Guid? UserIdentityId,
    Guid? OrganizationId,
    MembershipRole? Role,
    OrganizationContextFailure Failure)
{
    public bool Succeeded =>
        Failure == OrganizationContextFailure.None &&
        UserIdentityId.HasValue &&
        OrganizationId.HasValue &&
        Role.HasValue;

    public static OrganizationContextResolution Resolved(
        Guid userIdentityId,
        Guid organizationId,
        MembershipRole role) =>
        new(userIdentityId, organizationId, role, OrganizationContextFailure.None);

    public static OrganizationContextResolution Rejected(OrganizationContextFailure failure) =>
        new(null, null, null, failure);
}

public interface IOrganizationContextResolver
{
    Task<OrganizationContextResolution> ResolveAsync(
        string issuer,
        string subject,
        Guid? selectedOrganizationId,
        CancellationToken cancellationToken = default);
}

public sealed class EfOrganizationContextResolver(MoniterDbContext dbContext) : IOrganizationContextResolver
{
    public async Task<OrganizationContextResolution> ResolveAsync(
        string issuer,
        string subject,
        Guid? selectedOrganizationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(issuer) ||
            issuer.Length > 255 ||
            string.IsNullOrWhiteSpace(subject) ||
            subject.Length > 255)
        {
            return OrganizationContextResolution.Rejected(OrganizationContextFailure.IdentityNotProvisioned);
        }

        var identityIds = await dbContext.UserIdentities
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(identity =>
                identity.IsActive &&
                identity.Issuer == issuer &&
                identity.Subject == subject)
            .Select(identity => identity.Id)
            .Take(2)
            .ToListAsync(cancellationToken);

        if (identityIds.Count != 1)
        {
            return OrganizationContextResolution.Rejected(OrganizationContextFailure.IdentityNotProvisioned);
        }

        var identityId = identityIds[0];
        var activeMemberships = await dbContext.OrganizationMemberships
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(membership =>
                membership.UserIdentityId == identityId &&
                membership.Status == MembershipStatus.Active &&
                dbContext.Organizations.IgnoreQueryFilters().Any(organization =>
                    organization.Id == membership.OrganizationId &&
                    organization.Status == OrganizationStatus.Active))
            .Select(membership => new ActiveMembership(membership.OrganizationId, membership.Role))
            .ToListAsync(cancellationToken);

        if (selectedOrganizationId.HasValue)
        {
            var selectedMembership = activeMemberships
                .Where(membership => membership.OrganizationId == selectedOrganizationId.Value)
                .Take(2)
                .ToArray();

            return selectedMembership.Length == 1
                ? OrganizationContextResolution.Resolved(identityId, selectedMembership[0].OrganizationId, selectedMembership[0].Role)
                : OrganizationContextResolution.Rejected(OrganizationContextFailure.NoActiveMembership);
        }

        return activeMemberships.Count switch
        {
            0 => OrganizationContextResolution.Rejected(OrganizationContextFailure.NoActiveMembership),
            1 => OrganizationContextResolution.Resolved(
                identityId,
                activeMemberships[0].OrganizationId,
                activeMemberships[0].Role),
            _ => OrganizationContextResolution.Rejected(OrganizationContextFailure.OrganizationSelectionRequired)
        };
    }

    private sealed record ActiveMembership(Guid OrganizationId, MembershipRole Role);
}
