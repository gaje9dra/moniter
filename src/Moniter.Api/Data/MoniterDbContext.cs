using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data.Entities;
using Moniter.Api.Data.Tenancy;

namespace Moniter.Api.Data;

public sealed class MoniterDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public MoniterDbContext(
        DbContextOptions<MoniterDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public Guid? CurrentOrganizationId => _tenantContext.OrganizationId;

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();

    public DbSet<OrganizationMembership> OrganizationMemberships => Set<OrganizationMembership>();

    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MoniterModelConfiguration.Configure(modelBuilder);

        // Global filters are defense in depth. Repositories also require the
        // current tenant and apply explicit organization predicates.
        modelBuilder.Entity<Organization>()
            .HasQueryFilter(entity =>
                CurrentOrganizationId.HasValue && entity.Id == CurrentOrganizationId.Value);
        modelBuilder.Entity<OrganizationMembership>()
            .HasQueryFilter(entity =>
                CurrentOrganizationId.HasValue && entity.OrganizationId == CurrentOrganizationId.Value);
        modelBuilder.Entity<Department>()
            .HasQueryFilter(entity =>
                CurrentOrganizationId.HasValue && entity.OrganizationId == CurrentOrganizationId.Value);
    }

    public override int SaveChanges()
    {
        return SaveChanges(acceptAllChangesOnSuccess: true);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PrepareTrackedChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        PrepareTrackedChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void PrepareTrackedChanges()
    {
        var changedEntries = ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        var tenantOwnedEntries = changedEntries
            .Where(entry => entry.Entity is ITenantOwned)
            .ToArray();

        if (tenantOwnedEntries.Length > 0)
        {
            var currentOrganizationId = _tenantContext.GetRequiredOrganizationId();
            foreach (var entry in tenantOwnedEntries)
            {
                var tenantOwnedEntity = (ITenantOwned)entry.Entity;
                if (tenantOwnedEntity.OrganizationId != currentOrganizationId)
                {
                    throw new InvalidOperationException(
                        "A tenant-owned entity cannot be written outside the current organization scope.");
                }
            }
        }

        var now = DateTimeOffset.UtcNow;
        foreach (var entry in changedEntries)
        {
            if (entry.Entity is not ITimestampedEntity timestampedEntity)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                timestampedEntity.CreatedAt = now;
                timestampedEntity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                timestampedEntity.UpdatedAt = now;
            }
        }
    }
}
