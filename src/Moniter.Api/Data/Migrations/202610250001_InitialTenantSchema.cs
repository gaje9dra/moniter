using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Moniter.Api.Data;

namespace Moniter.Api.Data.Migrations;

[DbContext(typeof(MoniterDbContext))]
[Migration("202610250001_InitialTenantSchema")]
public sealed class InitialTenantSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "organizations",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                slug = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_organizations", item => item.id);
            });

        migrationBuilder.CreateTable(
            name: "user_identities",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                issuer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                display_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_identities", item => item.id);
            });

        migrationBuilder.CreateTable(
            name: "organization_memberships",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_identity_id = table.Column<Guid>(type: "uuid", nullable: false),
                role = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_organization_memberships", item => item.id);
                table.ForeignKey(
                    name: "fk_organization_memberships_organizations",
                    column: item => item.organization_id,
                    principalTable: "organizations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_organization_memberships_user_identities",
                    column: item => item.user_identity_id,
                    principalTable: "user_identities",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "departments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                parent_department_id = table.Column<Guid>(type: "uuid", nullable: true),
                name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_departments", item => item.id);
                table.UniqueConstraint(
                    name: "ak_departments_organization_id_id",
                    columns: item => new { item.organization_id, item.id });
                table.ForeignKey(
                    name: "fk_departments_organizations",
                    column: item => item.organization_id,
                    principalTable: "organizations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_departments_parent_same_organization",
                    columns: item => new { item.organization_id, item.parent_department_id },
                    principalTable: "departments",
                    principalColumns: new[] { "organization_id", "id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ux_organizations_slug",
            table: "organizations",
            column: "slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ux_user_identities_issuer_subject",
            table: "user_identities",
            columns: new[] { "issuer", "subject" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ux_organization_memberships_org_user",
            table: "organization_memberships",
            columns: new[] { "organization_id", "user_identity_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_organization_memberships_org_status",
            table: "organization_memberships",
            columns: new[] { "organization_id", "status" });

        migrationBuilder.CreateIndex(
            name: "ux_departments_org_name",
            table: "departments",
            columns: new[] { "organization_id", "name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_departments_org_parent",
            table: "departments",
            columns: new[] { "organization_id", "parent_department_id" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "departments");
        migrationBuilder.DropTable(name: "organization_memberships");
        migrationBuilder.DropTable(name: "user_identities");
        migrationBuilder.DropTable(name: "organizations");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 63);
        MoniterModelConfiguration.Configure(modelBuilder);
    }
}
