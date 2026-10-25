using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data;
using Moniter.Api.Data.Entities;
using Moniter.Api.Data.Tenancy;
using Moniter.Api.Security;
using Xunit;

namespace Moniter.Api.Tests;

[Collection("PostgreSQL tenant isolation")]
public sealed class OrganizationContextResolverTests
{
    [PostgreSqlFact]
    public async Task Resolver_requires_persisted_active_membership_and_explicit_selection_for_multiple_organizations()
    {
        var connectionString = Environment.GetEnvironmentVariable("MONITER_TEST_DATABASE")!;
        var options = new DbContextOptionsBuilder<MoniterDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        var issuer = "https://issuer.example.test";
        var subject = Guid.NewGuid().ToString("N");
        var organizationA = new Organization
        {
            Name = "Resolver Tenant A",
            Slug = "resolver-a-" + Guid.NewGuid().ToString("N")
        };
        var organizationB = new Organization
        {
            Name = "Resolver Tenant B",
            Slug = "resolver-b-" + Guid.NewGuid().ToString("N")
        };
        var identity = new UserIdentity
        {
            Issuer = issuer,
            Subject = subject,
            IsActive = true
        };

        await using (var setup = new MoniterDbContext(options, new TestTenantContext(null)))
        {
            await setup.Database.MigrateAsync();
            setup.Organizations.AddRange(organizationA, organizationB);
            setup.UserIdentities.Add(identity);
            await setup.SaveChangesAsync();
        }

        await using (var tenantA = new MoniterDbContext(options, new TestTenantContext(organizationA.Id)))
        {
            tenantA.OrganizationMemberships.Add(new OrganizationMembership
            {
                OrganizationId = organizationA.Id,
                UserIdentityId = identity.Id,
                Role = MembershipRole.Manager,
                Status = MembershipStatus.Active
            });
            await tenantA.SaveChangesAsync();
        }

        await using (var tenantB = new MoniterDbContext(options, new TestTenantContext(organizationB.Id)))
        {
            tenantB.OrganizationMemberships.Add(new OrganizationMembership
            {
                OrganizationId = organizationB.Id,
                UserIdentityId = identity.Id,
                Role = MembershipRole.Member,
                Status = MembershipStatus.Active
            });
            await tenantB.SaveChangesAsync();
        }

        await using var resolverContext = new MoniterDbContext(options, new TestTenantContext(null));
        var resolver = new EfOrganizationContextResolver(resolverContext);

        var noSelection = await resolver.ResolveAsync(issuer, subject, null);
        Assert.Equal(OrganizationContextFailure.OrganizationSelectionRequired, noSelection.Failure);

        var selectedA = await resolver.ResolveAsync(issuer, subject, organizationA.Id);
        Assert.True(selectedA.Succeeded);
        Assert.Equal(identity.Id, selectedA.UserIdentityId);
        Assert.Equal(organizationA.Id, selectedA.OrganizationId);
        Assert.Equal(MembershipRole.Manager, selectedA.Role);

        var unauthorized = await resolver.ResolveAsync(issuer, subject, Guid.NewGuid());
        Assert.Equal(OrganizationContextFailure.NoActiveMembership, unauthorized.Failure);

        var unknownIdentity = await resolver.ResolveAsync(issuer, Guid.NewGuid().ToString("N"), organizationA.Id);
        Assert.Equal(OrganizationContextFailure.IdentityNotProvisioned, unknownIdentity.Failure);
    }

    private sealed class TestTenantContext(Guid? organizationId) : ITenantContext
    {
        public Guid? OrganizationId { get; } = organizationId;
    }
}
