using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class DomainUserConfiguration : IEntityTypeConfiguration<DomainUser>
{
    public void Configure(EntityTypeBuilder<DomainUser> entityTypeBuilder)
    {
        // Email is optional for DomainUser (Value Object owned)
        entityTypeBuilder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired(false);

            email.HasIndex(e => e.Value);
        });

        // ApplicationUserId
        entityTypeBuilder.Property(u => u.ApplicationUserId)
            .IsRequired()
            .HasMaxLength(450); // ASP.NET Identity GUID string

        // Role
        entityTypeBuilder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        // Phone (optional)
        entityTypeBuilder.Property(u => u.Phone)
            .HasMaxLength(20);

        // BranchId is non-nullable Guid (SuperAdmin uses SYSTEM_BRANCH_ID)
        // No configuration needed - Guid is already non-nullable

        // Indexes
        entityTypeBuilder.HasIndex(u => u.ApplicationUserId);
        entityTypeBuilder.HasIndex(u => u.Role);
        entityTypeBuilder.HasIndex(u => u.BranchId);

        // Relationship: DomainUser -> Branch (optional)
        entityTypeBuilder
            .HasOne(u => u.Branch)
            .WithMany()
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
