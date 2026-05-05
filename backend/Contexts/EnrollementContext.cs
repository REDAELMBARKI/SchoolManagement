using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Contexts;

public class EnrollementContext : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder )
    {
        builder
        .HasOne(e => e.Student)
        .WithMany(s =>  s.Enrollments)
        .HasForeignKey(e => e.StudentId);

        builder
        .HasOne(e => e.Subject)
        .WithMany(s =>  s.Enrollments)
        .HasForeignKey(e => e.SubjectId);

        builder
        .HasOne(e => e.Branch)
        .WithMany(b =>  b.Enrollments)
        .HasForeignKey(e => e.BranchId);

        builder
        .HasOne(e => e.Plan)
        .WithMany(p =>  p.Enrollments)
        .HasForeignKey(e => e.PlanId);

        builder
        .HasOne(e => e.Group)
        .WithMany(g =>  g.Enrollments)
        .HasForeignKey(e => e.GroupId);

    }


}
