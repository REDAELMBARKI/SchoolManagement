using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> entityTypeBuilder)
    {
        // Table mapping for Teacher entity (inherits from Employee using TPT)
        entityTypeBuilder.ToTable("Teachers");
        
        // Teacher-specific properties
        entityTypeBuilder.Property(t => t.Specialization)
            .IsRequired()
            .HasMaxLength(100);
            
        // Index for Teacher-specific property
        entityTypeBuilder.HasIndex(t => t.Specialization);
      
    }
}
