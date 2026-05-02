using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entityTypeBuilder)
    {
        // TPC mapping for User entity
        entityTypeBuilder.ToTable("Users");
        
        // Property validations
        entityTypeBuilder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        entityTypeBuilder.Property(u => u.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        entityTypeBuilder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(u => u.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(u => u.Password)
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(u => u.IsActivated)
            .IsRequired()
            .HasDefaultValue(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(u => u.Email).IsUnique();
        entityTypeBuilder.HasIndex(u => u.Phone);
        entityTypeBuilder.HasIndex(u => u.Slug);
        entityTypeBuilder.HasIndex(u => u.IsActivated);
        
        // Check constraints
        entityTypeBuilder.ToTable("Users", tb =>
        {
            tb.HasCheckConstraint("CK_User_Email", "Email LIKE '%@%.%'");
            tb.HasCheckConstraint("CK_User_Phone", "Phone IS NOT NULL AND LEN(Phone) >= 10");
        });
    }
}
