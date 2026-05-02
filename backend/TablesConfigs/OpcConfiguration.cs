using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class OpcConfiguration : IEntityTypeConfiguration<Opc>
{
    public void Configure(EntityTypeBuilder<Opc> entityTypeBuilder)
    {
        // Table mapping for Opc entity (TPC inherited from Person)
        entityTypeBuilder.ToTable("Opcs");
        
        // Property validations
        entityTypeBuilder.Property(o => o.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(o => o.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(o => o.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(o => o.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(o => o.HireDate)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        entityTypeBuilder.Property(o => o.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
            
        entityTypeBuilder.Property(o => o.BranchId)
            .IsRequired();
            
        // Indexes
        entityTypeBuilder.HasIndex(o => o.Email).IsUnique();
        entityTypeBuilder.HasIndex(o => o.Phone);
        entityTypeBuilder.HasIndex(o => o.Slug);
        entityTypeBuilder.HasIndex(o => o.HireDate);
        entityTypeBuilder.HasIndex(o => o.BranchId);
        
        // Check constraints
        entityTypeBuilder.ToTable("Opcs", tb =>
        {
            tb.HasCheckConstraint("CK_Opc_Salary", "Salary > 0");
            tb.HasCheckConstraint("CK_Opc_HireDate", "HireDate <= GETDATE()");
            tb.HasCheckConstraint("CK_Opc_Email", "Email LIKE '%@%.%'");
        });
    }
}
