using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> entityTypeBuilder)
    {
        // TPT mapping for Employee entity (root of Employee hierarchy)
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
            
        // Email is optional for Employees
        entityTypeBuilder.Property(e => e.Email)
            .IsRequired(false)
            .HasMaxLength(255);
            
        // Phone is required for Employees
        entityTypeBuilder.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        // DateOfBirth is optional for Employees
        entityTypeBuilder.Property(e => e.DateOfBirth)
            .IsRequired(false);
            
        // GenderId is optional for Employees
        entityTypeBuilder.Property(e => e.GenderId)
            .IsRequired(false);
            
        entityTypeBuilder.Property(e => e.HireDate)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        entityTypeBuilder.Property(e => e.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
            
        entityTypeBuilder.Property(e => e.BranchId)
            .IsRequired();
            
        // Indexes
        entityTypeBuilder.HasIndex(e => e.Email).IsUnique();
        entityTypeBuilder.HasIndex(e => e.Phone);
        entityTypeBuilder.HasIndex(e => e.DateOfBirth);
        entityTypeBuilder.HasIndex(e => e.GenderId);
        entityTypeBuilder.HasIndex(e => e.Slug);
        entityTypeBuilder.HasIndex(e => e.HireDate);
        entityTypeBuilder.HasIndex(e => e.BranchId);
        
        // Check constraints
        entityTypeBuilder.ToTable("Employees", tb =>
        {
            tb.HasCheckConstraint("CK_Employee_Salary", "Salary > 0");
            tb.HasCheckConstraint("CK_Employee_HireDate", "HireDate <= datetime('now')");
            tb.HasCheckConstraint("CK_Employee_DateOfBirth", "DateOfBirth IS NULL OR DateOfBirth < datetime('now')");
        });
    }
}
