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
            
        // Email is required for Users
        entityTypeBuilder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(u => u.Phone)
            .IsRequired(false)
            .HasMaxLength(20);

        // DateOfBirth is optional for Users
        entityTypeBuilder.Property(u => u.DateOfBirth)
            .IsRequired(false);

        entityTypeBuilder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(u => u.IsActivated)
            .IsRequired()
            .HasDefaultValue(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(u => u.Email).IsUnique();
        entityTypeBuilder.HasIndex(u => u.Phone);
        entityTypeBuilder.HasIndex(u => u.DateOfBirth);
        entityTypeBuilder.HasIndex(u => u.Slug);
        entityTypeBuilder.HasIndex(u => u.IsActivated);
        
        // Check constraints
        entityTypeBuilder.ToTable("Users", tb =>
        {
            tb.HasCheckConstraint("CK_User_Email", "Email LIKE '%@%.%'");
            tb.HasCheckConstraint("CK_User_Phone", "Phone IS NULL OR LEN(Phone) >= 10");
            tb.HasCheckConstraint("CK_User_DateOfBirth", "DateOfBirth IS NULL OR DateOfBirth < GETDATE()");
        });
    }
}
