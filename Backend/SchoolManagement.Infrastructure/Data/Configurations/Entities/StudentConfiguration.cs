using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> entityTypeBuilder)
    {
        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
                
  
        // Email is optional for Students (Value Object owned)
        entityTypeBuilder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired(false);

            email.HasIndex(e => e.Value);
        });
            
        // Phone is required for Students
        entityTypeBuilder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        // DateOfBirth is required for Students
        entityTypeBuilder.Property(s => s.DateOfBirth)
            .IsRequired();

        // CreditBalance
        entityTypeBuilder.Property(s => s.CreditBalance)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

            
        entityTypeBuilder.Property(s => s.IntakeId)
            .IsRequired(false);
            
        // Indexes
        entityTypeBuilder.HasIndex(s => s.Phone);
        entityTypeBuilder.HasIndex(s => s.DateOfBirth);
        


        // relashioships 
        entityTypeBuilder
        .HasOne(s => s.Intake)
        .WithMany(i => i.Students)
        .HasForeignKey(s => s.IntakeId);
    }
}
