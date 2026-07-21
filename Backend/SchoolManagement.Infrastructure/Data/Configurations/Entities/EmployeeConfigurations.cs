using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;



public class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> entityTypeBuilder)
    {

        entityTypeBuilder.UseTpcMappingStrategy();

      
        // Configure Id for auto-increment for TPC
        entityTypeBuilder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entityTypeBuilder.UseTpcMappingStrategy();
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
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");


        entityTypeBuilder.Property(e => e.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
            
        entityTypeBuilder.Property(e => e.BranchId)
            .IsRequired();
            
        // Indexes
       
        entityTypeBuilder.HasIndex(e => e.Phone);
        entityTypeBuilder.HasIndex(e => e.DateOfBirth);
        entityTypeBuilder.HasIndex(e => e.GenderId);
        entityTypeBuilder.HasIndex(e => e.Slug);
        entityTypeBuilder.HasIndex(e => e.HireDate);
        entityTypeBuilder.HasIndex(e => e.BranchId);
        
       
    }
}
