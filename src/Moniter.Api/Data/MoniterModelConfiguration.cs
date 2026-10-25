using Microsoft.EntityFrameworkCore;
using Moniter.Api.Data.Entities;

namespace Moniter.Api.Data;

internal static class MoniterModelConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("organizations");
            entity.HasKey(item => item.Id).HasName("pk_organizations");
            entity.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            entity.Property(item => item.Slug).HasColumnName("slug").HasMaxLength(63).IsRequired();
            entity.Property(item => item.Status).HasColumnName("status")
                .HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(item => item.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.HasIndex(item => item.Slug).IsUnique().HasDatabaseName("ux_organizations_slug");
        });

        modelBuilder.Entity<UserIdentity>(entity =>
        {
            entity.ToTable("user_identities");
            entity.HasKey(item => item.Id).HasName("pk_user_identities");
            entity.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(item => item.Issuer).HasColumnName("issuer").HasMaxLength(255).IsRequired();
            entity.Property(item => item.Subject).HasColumnName("subject").HasMaxLength(255).IsRequired();
            entity.Property(item => item.Email).HasColumnName("email").HasMaxLength(320);
            entity.Property(item => item.DisplayName).HasColumnName("display_name").HasMaxLength(160);
            entity.Property(item => item.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(item => item.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.HasIndex(item => new { item.Issuer, item.Subject })
                .IsUnique().HasDatabaseName("ux_user_identities_issuer_subject");
        });

        modelBuilder.Entity<OrganizationMembership>(entity =>
        {
            entity.ToTable("organization_memberships");
            entity.HasKey(item => item.Id).HasName("pk_organization_memberships");
            entity.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(item => item.OrganizationId).HasColumnName("organization_id").IsRequired();
            entity.Property(item => item.UserIdentityId).HasColumnName("user_identity_id").IsRequired();
            entity.Property(item => item.Role).HasColumnName("role")
                .HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(item => item.Status).HasColumnName("status")
                .HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(item => item.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.HasIndex(item => new { item.OrganizationId, item.UserIdentityId })
                .IsUnique().HasDatabaseName("ux_organization_memberships_org_user");
            entity.HasIndex(item => new { item.OrganizationId, item.Status })
                .HasDatabaseName("ix_organization_memberships_org_status");
            entity.HasOne(item => item.Organization)
                .WithMany(item => item.Memberships)
                .HasForeignKey(item => item.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_organization_memberships_organizations");
            entity.HasOne(item => item.UserIdentity)
                .WithMany(item => item.Memberships)
                .HasForeignKey(item => item.UserIdentityId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_organization_memberships_user_identities");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasKey(item => item.Id).HasName("pk_departments");
            entity.HasAlternateKey(item => new { item.OrganizationId, item.Id })
                .HasName("ak_departments_organization_id_id");
            entity.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(item => item.OrganizationId).HasColumnName("organization_id").IsRequired();
            entity.Property(item => item.ParentDepartmentId).HasColumnName("parent_department_id");
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            entity.Property(item => item.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.HasIndex(item => new { item.OrganizationId, item.Name })
                .IsUnique().HasDatabaseName("ux_departments_org_name");
            entity.HasIndex(item => new { item.OrganizationId, item.ParentDepartmentId })
                .HasDatabaseName("ix_departments_org_parent");
            entity.HasOne(item => item.Organization)
                .WithMany(item => item.Departments)
                .HasForeignKey(item => item.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_departments_organizations");
            entity.HasOne(item => item.ParentDepartment)
                .WithMany(item => item.Children)
                .HasForeignKey(item => new { item.OrganizationId, item.ParentDepartmentId })
                .HasPrincipalKey(item => new { item.OrganizationId, item.Id })
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_departments_parent_same_organization");
        });
    }
}
