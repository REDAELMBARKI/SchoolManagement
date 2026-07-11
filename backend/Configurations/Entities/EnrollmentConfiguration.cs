using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

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



        // relationships
      
        entityTypeBuilder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        entityTypeBuilder
        .HasOne(e => e.Subject)
        .WithMany(s =>  s.Enrollments)
        .HasForeignKey(e => e.SubjectId);

        entityTypeBuilder
        .HasOne(e => e.Branch)
        .WithMany(b =>  b.Enrollments)
        .HasForeignKey(e => e.BranchId)
         .OnDelete(DeleteBehavior.Restrict);

        entityTypeBuilder
        .HasOne(e => e.Plan)
        .WithMany(p =>  p.Enrollments)
        .HasForeignKey(e => e.PlanId)
         .OnDelete(DeleteBehavior.Restrict);

        entityTypeBuilder
        .HasOne(e => e.Group)
        .WithMany(g =>  g.Enrollments)
        .HasForeignKey(e => e.GroupId)
         .OnDelete(DeleteBehavior.Restrict);

    }
}
