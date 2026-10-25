using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Moniter.Api.Data;
using Moniter.Api.Data.Entities;

namespace Moniter.Api.Data.Migrations;

[DbContext(typeof(MoniterDbContext))]
[Migration("202610250002_AddMembershipUserIdentityIndex")]
public sealed class AddMembershipUserIdentityIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_organization_memberships_user_identity_id",
            table: "organization_memberships",
            column: "user_identity_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_organization_memberships_user_identity_id",
            table: "organization_memberships");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 63);
        MoniterModelConfiguration.Configure(modelBuilder);
        modelBuilder.Entity<OrganizationMembership>()
            .HasIndex(item => item.UserIdentityId)
            .HasDatabaseName("IX_organization_memberships_user_identity_id");
    }
}
