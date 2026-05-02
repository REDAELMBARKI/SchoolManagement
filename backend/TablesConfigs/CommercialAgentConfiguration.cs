using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class CommercialAgentConfiguration : IEntityTypeConfiguration<CommercialAgent>
{
    public void Configure(EntityTypeBuilder<CommercialAgent> entityTypeBuilder)
    {
        // Table mapping for CommercialAgent entity (TPC inherited from Person)
        entityTypeBuilder.ToTable("CommercialAgents");
        
        // Property validations
        entityTypeBuilder.Property(ca => ca.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(ca => ca.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(ca => ca.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(ca => ca.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(ca => ca.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(ca => ca.HireDate)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        entityTypeBuilder.Property(ca => ca.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
            
        entityTypeBuilder.Property(ca => ca.BranchId)
            .IsRequired();
            
        // Indexes
        entityTypeBuilder.HasIndex(ca => ca.Email).IsUnique();
        entityTypeBuilder.HasIndex(ca => ca.Phone);
        entityTypeBuilder.HasIndex(ca => ca.Slug);
        entityTypeBuilder.HasIndex(ca => ca.HireDate);
        entityTypeBuilder.HasIndex(ca => ca.BranchId);
        
        // Check constraints
        entityTypeBuilder.ToTable("CommercialAgents", tb =>
        {
            tb.HasCheckConstraint("CK_CommercialAgent_Salary", "Salary > 0");
            tb.HasCheckConstraint("CK_CommercialAgent_HireDate", "HireDate <= GETDATE()");
            tb.HasCheckConstraint("CK_CommercialAgent_Email", "Email LIKE '%@%.%'");
        });
    }
}
