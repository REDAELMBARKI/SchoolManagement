using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> entityTypeBuilder)
    {
        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(t => t.Id)
            .ValueGeneratedOnAdd();
                
        // Table mapping for Teacher entity (TPC inherited from Employee)
        entityTypeBuilder.ToTable("Teachers", tb =>
        {
            tb.HasCheckConstraint("CK_Teacher_Salary", "Salary > 0");
        });

        // Teacher-specific properties
        entityTypeBuilder.Property(t => t.Specialization)
            .IsRequired()
            .HasMaxLength(100);
            
        // Index for Teacher-specific property
        entityTypeBuilder.HasIndex(t => t.Specialization);
      
    }
}
