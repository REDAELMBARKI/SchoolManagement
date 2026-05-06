using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> entityTypeBuilder)
    {
        entityTypeBuilder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        entityTypeBuilder.Property(g => g.Period)
            .IsRequired()
            .HasMaxLength(20);

        entityTypeBuilder.Property(g => g.Capacity)
            .IsRequired();

        entityTypeBuilder.Property(g => g.BranchId)
            .IsRequired();

        entityTypeBuilder.Property(g => g.LevelId)
            .IsRequired();

        entityTypeBuilder.Property(g => g.LanguageId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(g => g.BranchId);
        entityTypeBuilder.HasIndex(g => g.LevelId);
        entityTypeBuilder.HasIndex(g => g.LanguageId);

        // Group → Branch relationship
        entityTypeBuilder.HasOne(g => g.Branch)
            .WithMany()
            .HasForeignKey(g => g.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Group → Level relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(g => g.Level)
            .WithMany()
            .HasForeignKey(g => g.LevelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Group → Subject relationship (inferred from LanguageId)
        entityTypeBuilder.HasOne(g => g.Subject)
            .WithMany()
            .HasForeignKey(g => g.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
