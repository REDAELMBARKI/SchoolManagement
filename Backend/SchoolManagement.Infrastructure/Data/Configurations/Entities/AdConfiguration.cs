using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        builder.Property(a => a.Name)
            .IsRequired();

        builder.Property(a => a.Slug)
            .IsRequired();

        builder.Property(a => a.PlatformId)
            .IsRequired();

        builder.Property(a => a.BranchId)
            .IsRequired();

        // Foreign key to Platform - NoAction (can't delete platform if it has ads)
        builder.HasOne(a => a.Platform)
            .WithMany()
            .HasForeignKey(a => a.PlatformId)
            .OnDelete(DeleteBehavior.NoAction);

        // Foreign key to Branch - Cascade (if branch deleted, delete its ads)
        builder.HasOne(a => a.Branch)
            .WithMany()
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(a => a.Slug);
        builder.HasIndex(a => a.PlatformId);
        builder.HasIndex(a => a.BranchId);
    }
}
