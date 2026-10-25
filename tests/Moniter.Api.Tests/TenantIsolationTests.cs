using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data;
using Moniter.Api.Data.Entities;
using Moniter.Api.Data.Repositories;
using Moniter.Api.Data.Tenancy;
using Xunit;

namespace Moniter.Api.Tests;

[CollectionDefinition("PostgreSQL tenant isolation", DisableParallelization = true)]
public sealed class PostgreSqlTenantIsolationCollection;

[Collection("PostgreSQL tenant isolation")]
public sealed class TenantIsolationTests
{
    [PostgreSqlFact]
    public async Task Tenant_repositories_and_database_constraints_isolate_organizations()
    {
        var connectionString = Environment.GetEnvironmentVariable("MONITER_TEST_DATABASE")!;
        var options = new DbContextOptionsBuilder<MoniterDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var organizationA = new Organization
        {
            Name = "Tenant A",
            Slug = "tenant-a-" + Guid.NewGuid().ToString("N")
        };
        var organizationB = new Organization
        {
            Name = "Tenant B",
            Slug = "tenant-b-" + Guid.NewGuid().ToString("N")
        };
        var identity = new UserIdentity
        {
            Issuer = "https://issuer.example.test",
            Subject = Guid.NewGuid().ToString("N"),
            Email = "test@example.test",
            DisplayName = "Integration Test User"
        };

        await using (var setup = new MoniterDbContext(options, new TestTenantContext(null)))
        {
            await setup.Database.MigrateAsync();
            setup.Organizations.AddRange(organizationA, organizationB);
            setup.UserIdentities.Add(identity);
            await setup.SaveChangesAsync();
        }

        var departmentA = new Department { OrganizationId = organizationA.Id, Name = "Engineering" };
        var membershipA = new OrganizationMembership
        {
            OrganizationId = organizationA.Id,
            UserIdentityId = identity.Id,
            Role = MembershipRole.Administrator,
            Status = MembershipStatus.Active
        };
        await using (var tenantA = new MoniterDbContext(options, new TestTenantContext(organizationA.Id)))
        {
            tenantA.Departments.Add(departmentA);
            tenantA.OrganizationMemberships.Add(membershipA);
            await tenantA.SaveChangesAsync();
        }

        var departmentB = new Department { OrganizationId = organizationB.Id, Name = "Finance" };
        var membershipB = new OrganizationMembership
        {
            OrganizationId = organizationB.Id,
            UserIdentityId = identity.Id,
            Role = MembershipRole.Member,
            Status = MembershipStatus.Active
        };
        await using (var tenantB = new MoniterDbContext(options, new TestTenantContext(organizationB.Id)))
        {
            tenantB.Departments.Add(departmentB);
            tenantB.OrganizationMemberships.Add(membershipB);
            await tenantB.SaveChangesAsync();
        }

        await using (var tenantA = new MoniterDbContext(options, new TestTenantContext(organizationA.Id)))
        {
            var departmentRepository = new EfDepartmentRepository(tenantA, new TestTenantContext(organizationA.Id));
            var membershipRepository = new EfOrganizationMembershipRepository(
                tenantA,
                new TestTenantContext(organizationA.Id));

            var departments = await departmentRepository.ListCurrentAsync();
            Assert.Single(departments);
            Assert.Equal(departmentA.Id, departments[0].Id);
            Assert.Null(await departmentRepository.FindCurrentAsync(departmentB.Id));

            var memberships = await membershipRepository.ListCurrentAsync();
            Assert.Single(memberships);
            Assert.Equal(organizationA.Id, memberships[0].OrganizationId);
            Assert.Equal(identity.Id, memberships[0].UserIdentityId);
        }

        await using (var tenantA = new MoniterDbContext(options, new TestTenantContext(organizationA.Id)))
        {
            tenantA.Departments.Add(new Department
            {
                OrganizationId = organizationA.Id,
                ParentDepartmentId = departmentB.Id,
                Name = "Invalid cross-tenant child"
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => tenantA.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task Tenant_owned_writes_are_rejected_without_a_trusted_tenant()
    {
        var options = new DbContextOptionsBuilder<MoniterDbContext>().Options;
        await using var dbContext = new MoniterDbContext(options, new TestTenantContext(null));
        dbContext.Departments.Add(new Department { OrganizationId = Guid.NewGuid(), Name = "Not allowed" });

        await Assert.ThrowsAsync<TenantScopeRequiredException>(() => dbContext.SaveChangesAsync());
    }

    [Fact]
    public void Http_tenant_context_ignores_organization_claims_from_unauthenticated_principals()
    {
        var organizationId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(TenantClaimTypes.OrganizationId, organizationId.ToString())
            }))
        };
        var tenantContext = new HttpTenantContext(new HttpContextAccessor { HttpContext = httpContext });

        Assert.Null(tenantContext.OrganizationId);
    }

    [Fact]
    public void Http_tenant_context_reads_one_valid_claim_from_an_authenticated_principal()
    {
        var organizationId = Guid.NewGuid();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(TenantClaimTypes.OrganizationId, organizationId.ToString())
        }, "test-authentication");
        var tenantContext = new HttpTenantContext(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        });

        Assert.Equal(organizationId, tenantContext.OrganizationId);
    }

    private sealed class TestTenantContext(Guid? organizationId) : ITenantContext
    {
        public Guid? OrganizationId { get; } = organizationId;
    }
}
