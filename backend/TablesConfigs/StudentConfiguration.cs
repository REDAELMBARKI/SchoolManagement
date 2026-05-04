using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> entityTypeBuilder)
    {
        // Table mapping for Student entity (TPC inherited from Person)
        entityTypeBuilder.ToTable("Students");
        

            
        // Email is optional for Students
        entityTypeBuilder.Property(s => s.Email)
            .IsRequired(false)
            .HasMaxLength(255);
            
        // Phone is required for Students
        entityTypeBuilder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        // DateOfBirth is required for Students
        entityTypeBuilder.Property(s => s.DateOfBirth)
            .IsRequired();

        entityTypeBuilder.Property(s => s.LevelId)
            .IsRequired();
            
        entityTypeBuilder.Property(s => s.GroupId)
            .IsRequired();
            
        entityTypeBuilder.Property(s => s.IntakeId)
            .IsRequired(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(s => s.Email);
        entityTypeBuilder.HasIndex(s => s.Phone);
        entityTypeBuilder.HasIndex(s => s.DateOfBirth);
        entityTypeBuilder.HasIndex(s => s.LevelId);
        entityTypeBuilder.HasIndex(s => s.GroupId);
        
        // Check constraints
        entityTypeBuilder.ToTable("Students", tb =>
        {
            tb.HasCheckConstraint("CK_Student_DateOfBirth", "DateOfBirth < datetime('now')");
            tb.HasCheckConstraint("CK_Student_Email", "Email LIKE '%@%.%'");
        });
    }
}
