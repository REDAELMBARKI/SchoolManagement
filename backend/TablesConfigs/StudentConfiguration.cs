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
        
        // Property validations
        entityTypeBuilder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(s => s.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(s => s.DateOfBirth)
            .IsRequired(false);
            
        entityTypeBuilder.Property(s => s.LevelId)
            .IsRequired();
            
        entityTypeBuilder.Property(s => s.GroupId)
            .IsRequired(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(s => s.Email).IsUnique();
        entityTypeBuilder.HasIndex(s => s.Phone);
        entityTypeBuilder.HasIndex(s => s.Slug);
        entityTypeBuilder.HasIndex(s => s.LevelId);
        entityTypeBuilder.HasIndex(s => s.GroupId);
        entityTypeBuilder.HasIndex(s => s.DateOfBirth);
        
        // Check constraints
        entityTypeBuilder.ToTable("Students", tb =>
        {
            tb.HasCheckConstraint("CK_Student_DateOfBirth", "DateOfBirth IS NULL OR DateOfBirth < GETDATE()");
            tb.HasCheckConstraint("CK_Student_Email", "Email LIKE '%@%.%'");
        });
    }
}
