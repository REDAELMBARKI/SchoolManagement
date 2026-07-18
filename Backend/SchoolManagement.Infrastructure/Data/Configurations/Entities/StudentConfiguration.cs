using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> entityTypeBuilder)
    {
        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
                
  
        // Email is optional for Students
        entityTypeBuilder.Property(s => s.Email)
            .IsRequired(false)
            .HasMaxLength(255);
            
        // Phone is required for Students
        entityTypeBuilder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        // DateOfBirth is required for Students
        entityTypeBuilder.Property(s => s.DateOfBirth)
            .IsRequired();

            
        entityTypeBuilder.Property(s => s.IntakeId)
            .IsRequired(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(s => s.Email);
        entityTypeBuilder.HasIndex(s => s.Phone);
        entityTypeBuilder.HasIndex(s => s.DateOfBirth);
        


        // relashioships 
        entityTypeBuilder
        .HasOne(s => s.Intake)
        .WithOne(i => i.ConvertedToStudent)
        .HasForeignKey<Student>(s => s.IntakeId);
    }
}
