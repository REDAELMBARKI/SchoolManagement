using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Contexts;

public class StudentContext : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Student → Intake relationship (many students can come from one intake)
        builder
        .HasOne(s => s.Intake)
        .WithMany()
        .HasForeignKey(s => s.IntakeId);

    }
}
