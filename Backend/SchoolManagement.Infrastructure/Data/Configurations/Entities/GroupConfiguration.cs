using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

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

        entityTypeBuilder.Property(g => g.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        entityTypeBuilder.Property(g => g.BranchId)
            .IsRequired();

        entityTypeBuilder.Property(g => g.LevelId)
            .IsRequired();

        entityTypeBuilder.Property(g => g.SubjectId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(g => g.BranchId);
        entityTypeBuilder.HasIndex(g => g.LevelId);
        entityTypeBuilder.HasIndex(g => g.SubjectId);

        // Group → Branch relationship
        entityTypeBuilder.HasOne(g => g.Branch)
            .WithMany()
            .HasForeignKey(g => g.BranchId)
            .OnDelete(DeleteBehavior.NoAction);

        // Group → Level relationship (FIXED: Use NoAction to avoid cascade cycles)
        entityTypeBuilder.HasOne(g => g.Level)
            .WithMany()
            .HasForeignKey(g => g.LevelId)
            .OnDelete(DeleteBehavior.NoAction);

        // Group → Subject relationship (inferred from LanguageId)
        entityTypeBuilder.HasOne(g => g.Subject)
            .WithMany()
            .HasForeignKey(g => g.SubjectId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
