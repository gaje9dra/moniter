using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moniter.Api.Data;
using Moniter.Api.Data.Entities;

namespace Moniter.Api.Data.Migrations;

[DbContext(typeof(MoniterDbContext))]
public sealed class MoniterDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 63);
        MoniterModelConfiguration.Configure(modelBuilder);

        // The convention-created index required by the UserIdentity foreign key
        // is captured explicitly so the snapshot matches the runtime model.
        modelBuilder.Entity<OrganizationMembership>()
            .HasIndex(item => item.UserIdentityId)
            .HasDatabaseName("IX_organization_memberships_user_identity_id");
    }
}
