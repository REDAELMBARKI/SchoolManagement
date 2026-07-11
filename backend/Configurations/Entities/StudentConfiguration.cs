using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> entityTypeBuilder)
    {
        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
                
        // Table mapping for Student entity (TPC inherited from Person)
        entityTypeBuilder.ToTable("Students");
        

            
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
