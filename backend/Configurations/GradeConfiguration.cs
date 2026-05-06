using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> entityTypeBuilder)
    {
        entityTypeBuilder.Property(g => g.EvaluationType)
            .IsRequired()
            .HasMaxLength(50);

        entityTypeBuilder.Property(g => g.Comment)
            .HasMaxLength(300);

        entityTypeBuilder.Property(g => g.StudentId)
            .IsRequired();

        entityTypeBuilder.Property(g => g.GroupTeacherId)
            .IsRequired();

        entityTypeBuilder.Property(g => g.BranchId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(g => g.StudentId);
        entityTypeBuilder.HasIndex(g => g.GroupTeacherId);
        entityTypeBuilder.HasIndex(g => g.BranchId);

        // Grade → Student relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(g => g.Student)
            .WithMany()
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Grade → GroupTeacher relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(g => g.GroupTeacher)
            .WithMany()
            .HasForeignKey(g => g.GroupTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Grade → Branch relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(g => g.Branch)
            .WithMany()
            .HasForeignKey(g => g.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
