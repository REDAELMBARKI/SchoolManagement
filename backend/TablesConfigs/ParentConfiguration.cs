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

        // Email is optional for Parents
        entityTypeBuilder.Property(p => p.Email)
            .IsRequired(false)
            .HasMaxLength(255);
            
        // Phone is optional for Parents
        entityTypeBuilder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(p => p.Relationship)
            .IsRequired();

        // Indexes
        entityTypeBuilder.HasIndex(p => p.Email).IsUnique();
        entityTypeBuilder.HasIndex(p => p.Phone);
        entityTypeBuilder.HasIndex(p => p.Relationship);
        
        // Check constraints
        entityTypeBuilder.ToTable("Parents", tb =>
        {
            tb.HasCheckConstraint("CK_Parent_Email", "Email LIKE '%@%.%'");
            tb.HasCheckConstraint("CK_Parent_DateOfBirth", "DateOfBirth IS NULL OR DateOfBirth < datetime('now')");
        });
    }
}
