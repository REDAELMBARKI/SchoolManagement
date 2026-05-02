using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> entityTypeBuilder)
    {
        // Table mapping for Teacher entity (TPC inherited from Person)
        entityTypeBuilder.ToTable("Teachers");
        
        // Property validations
        entityTypeBuilder.Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(t => t.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(t => t.Specialization)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(t => t.HireDate)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        entityTypeBuilder.Property(t => t.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
            
        entityTypeBuilder.Property(t => t.BranchId)
            .IsRequired();
            
        // Indexes
        entityTypeBuilder.HasIndex(t => t.Email).IsUnique();
        entityTypeBuilder.HasIndex(t => t.Phone);
        entityTypeBuilder.HasIndex(t => t.Slug);
        entityTypeBuilder.HasIndex(t => t.Specialization);
        entityTypeBuilder.HasIndex(t => t.HireDate);
        entityTypeBuilder.HasIndex(t => t.BranchId);
        
        // Check constraints
        entityTypeBuilder.ToTable("Teachers", tb =>
        {
            tb.HasCheckConstraint("CK_Teacher_Salary", "Salary > 0");
            tb.HasCheckConstraint("CK_Teacher_HireDate", "HireDate <= GETDATE()");
            tb.HasCheckConstraint("CK_Teacher_Email", "Email LIKE '%@%.%'");
        });
    }
}
