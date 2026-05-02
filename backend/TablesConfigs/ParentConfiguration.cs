using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class ParentConfiguration : IEntityTypeConfiguration<Parent>
{
    public void Configure(EntityTypeBuilder<Parent> entityTypeBuilder)
    {
        // Table mapping for Parent entity (TPC inherited from Person)
        entityTypeBuilder.ToTable("Parents");
        
        // Property validations
        entityTypeBuilder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(p => p.Relationship)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(p => p.DateOfBirth)
            .IsRequired(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(p => p.Email).IsUnique();
        entityTypeBuilder.HasIndex(p => p.Phone);
        entityTypeBuilder.HasIndex(p => p.Slug);
        entityTypeBuilder.HasIndex(p => p.Relationship);
        
        // Check constraints
        entityTypeBuilder.ToTable("Parents", tb =>
        {
            tb.HasCheckConstraint("CK_Parent_Email", "Email LIKE '%@%.%'");
            tb.HasCheckConstraint("CK_Parent_DateOfBirth", "DateOfBirth IS NULL OR DateOfBirth < GETDATE()");
        });
    }
}
