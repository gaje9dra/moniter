using Moniter.Api.Data.Entities;

namespace Moniter.Api.Data.Repositories;

public interface IOrganizationRepository
{
    Task<Organization?> GetCurrentAsync(CancellationToken cancellationToken = default);
}

public interface IOrganizationMembershipRepository
{
    Task<IReadOnlyList<OrganizationMembership>> ListCurrentAsync(
        CancellationToken cancellationToken = default);

    Task<OrganizationMembership?> FindCurrentAsync(
        Guid membershipId,
        CancellationToken cancellationToken = default);
}

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> ListCurrentAsync(CancellationToken cancellationToken = default);

    Task<Department?> FindCurrentAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<Department> CreateCurrentAsync(
        string name,
        Guid? parentDepartmentId = null,
        CancellationToken cancellationToken = default);
}

public sealed class ParentDepartmentNotFoundException : InvalidOperationException
{
    public ParentDepartmentNotFoundException()
        : base("The parent department was not found in the current organization.")
    {
    }
}
