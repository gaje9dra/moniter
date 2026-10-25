using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Moniter.Api.Data.Tenancy;

namespace Moniter.Api.Data;

public sealed class MoniterDbContextFactory : IDesignTimeDbContextFactory<MoniterDbContext>
{
    public MoniterDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        var connectionString = DatabaseConnectionString.TryResolve(configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__Moniter or MONITER_DATABASE_URL before using EF Core design-time commands.");
        }

        var options = new DbContextOptionsBuilder<MoniterDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new MoniterDbContext(options, new DesignTimeTenantContext());
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid? OrganizationId => null;
    }
}
