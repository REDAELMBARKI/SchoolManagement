using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> entityTypeBuilder)
    {

        // Email is optional for Employees
        entityTypeBuilder
            .OwnsOne(emp => emp.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(100)
                    .IsRequired(false);
            });


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
