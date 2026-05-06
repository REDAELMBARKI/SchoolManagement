using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> entityTypeBuilder)
    {
        entityTypeBuilder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        entityTypeBuilder.Property(e => e.StudentId)
            .IsRequired();

        entityTypeBuilder.Property(e => e.GroupId)
            .IsRequired();

        entityTypeBuilder.Property(e => e.BranchId)
            .IsRequired();

        entityTypeBuilder.Property(e => e.PlanId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(e => e.StudentId);
        entityTypeBuilder.HasIndex(e => e.GroupId);
        entityTypeBuilder.HasIndex(e => e.BranchId);
        entityTypeBuilder.HasIndex(e => e.PlanId);

        // Enrollment → Student relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrollment → Group relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(e => e.Group)
            .WithMany()
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrollment → Branch relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(e => e.Branch)
            .WithMany()
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrollment → Plan relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(e => e.Plan)
            .WithMany()
            .HasForeignKey(e => e.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
