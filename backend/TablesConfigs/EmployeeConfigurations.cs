using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> entityTypeBuilder)
    {
        // TPC mapping for Employee entity
        entityTypeBuilder.UseTptMappingStrategy().ToTable("Employees");
        
        // Property validations
        entityTypeBuilder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(e => e.HireDate)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        entityTypeBuilder.Property(e => e.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
            
        entityTypeBuilder.Property(e => e.BranchId)
            .IsRequired();
            
        // Indexes
        entityTypeBuilder.HasIndex(e => e.Email).IsUnique();
        entityTypeBuilder.HasIndex(e => e.Phone);
        entityTypeBuilder.HasIndex(e => e.Slug);
        entityTypeBuilder.HasIndex(e => e.HireDate);
        entityTypeBuilder.HasIndex(e => e.BranchId);
        
        // Check constraints
        entityTypeBuilder.ToTable("Employees", tb =>
        {
            tb.HasCheckConstraint("CK_Employee_Salary", "Salary > 0");
            tb.HasCheckConstraint("CK_Employee_HireDate", "HireDate <= GETDATE()");
            tb.HasCheckConstraint("CK_Employee_Email", "Email LIKE '%@%.%'");
        });
    }
}
