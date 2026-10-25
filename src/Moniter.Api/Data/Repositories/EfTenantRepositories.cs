using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data;
using Moniter.Api.Data.Entities;
using Moniter.Api.Data.Tenancy;

namespace Moniter.Api.Data.Repositories;

public sealed class EfOrganizationRepository(
    MoniterDbContext dbContext,
    ITenantContext tenantContext) : IOrganizationRepository
{
    public Task<Organization?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var organizationId = tenantContext.GetRequiredOrganizationId();
        return dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(
                organization => organization.Id == organizationId,
                cancellationToken);
    }
}

public sealed class EfOrganizationMembershipRepository(
    MoniterDbContext dbContext,
    ITenantContext tenantContext) : IOrganizationMembershipRepository
{
    public async Task<IReadOnlyList<OrganizationMembership>> ListCurrentAsync(
        CancellationToken cancellationToken = default)
    {
        var organizationId = tenantContext.GetRequiredOrganizationId();
        return await dbContext.OrganizationMemberships
            .AsNoTracking()
            .Include(membership => membership.UserIdentity)
            .Where(membership => membership.OrganizationId == organizationId)
            .OrderBy(membership => membership.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<OrganizationMembership?> FindCurrentAsync(
        Guid membershipId,
        CancellationToken cancellationToken = default)
    {
        var organizationId = tenantContext.GetRequiredOrganizationId();
        return dbContext.OrganizationMemberships
            .AsNoTracking()
            .Include(membership => membership.UserIdentity)
            .SingleOrDefaultAsync(
                membership => membership.OrganizationId == organizationId && membership.Id == membershipId,
                cancellationToken);
    }
}

public sealed class EfDepartmentRepository(
    MoniterDbContext dbContext,
    ITenantContext tenantContext) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> ListCurrentAsync(
        CancellationToken cancellationToken = default)
    {
        var organizationId = tenantContext.GetRequiredOrganizationId();
        return await dbContext.Departments
            .AsNoTracking()
            .Where(department => department.OrganizationId == organizationId)
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Department?> FindCurrentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var organizationId = tenantContext.GetRequiredOrganizationId();
        return dbContext.Departments
            .AsNoTracking()
            .SingleOrDefaultAsync(
                department => department.OrganizationId == organizationId && department.Id == departmentId,
                cancellationToken);
    }

    public async Task<Department> CreateCurrentAsync(
        string name,
        Guid? parentDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        var organizationId = tenantContext.GetRequiredOrganizationId();
        var normalizedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length > 120)
        {
            throw new ArgumentException("Department name must contain 1 to 120 characters.", nameof(name));
        }

        if (parentDepartmentId.HasValue &&
            !await dbContext.Departments.AnyAsync(
                department =>
                    department.OrganizationId == organizationId &&
                    department.Id == parentDepartmentId.Value,
                cancellationToken))
        {
            throw new ParentDepartmentNotFoundException();
        }

        var department = new Department
        {
            OrganizationId = organizationId,
            ParentDepartmentId = parentDepartmentId,
            Name = normalizedName
        };

        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return department;
    }
}
