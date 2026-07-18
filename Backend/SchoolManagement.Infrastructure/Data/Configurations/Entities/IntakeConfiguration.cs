using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class IntakeConfiguration : IEntityTypeConfiguration<Intake>
{
    public void Configure(EntityTypeBuilder<Intake> entityTypeBuilder)
    {
        
 
        // Check constraints for business rules
        entityTypeBuilder.ToTable("Intakes", tb =>
        {
            tb.HasCheckConstraint("CK_Intake_FollowUpDate", "FollowUpDate IS NULL OR FollowUpDate >= IntakeDate");
            // NOTE: Date validation moved to C# logic/FluentValidation
        });



        // Email is optional for Intakes
        entityTypeBuilder.Property(i => i.Email)
            .IsRequired(false)
            .HasMaxLength(255);

        // Phone is optional for Intakes
        entityTypeBuilder.Property(i => i.Phone)
            .IsRequired(false)
            .HasMaxLength(20);

        // DateOfBirth is optional for Intakes
        entityTypeBuilder.Property(i => i.DateOfBirth)
            .IsRequired(false);

        // IntakeDate validation
        entityTypeBuilder.Property(i => i.IntakeDate)
            .IsRequired();

        // FollowUpDate validation
        entityTypeBuilder.Property(i => i.FollowUpDate)
            .IsRequired(false);

        // Status validation
        entityTypeBuilder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>();

    
        // Notes validation
        entityTypeBuilder.Property(i => i.Notes)
            .HasMaxLength(300);

        // Foreign key validations
        entityTypeBuilder.Property(i => i.CommercialAgentId)
            .IsRequired(false);

        entityTypeBuilder.Property(i => i.GenderId)
            .IsRequired(false);

        entityTypeBuilder.Property(i => i.LeadSourceId)
            .IsRequired(false);

        entityTypeBuilder.Property(i => i.SubjectId)
            .IsRequired();

        entityTypeBuilder.Property(i => i.BranchId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(i => i.Email).IsUnique();
        entityTypeBuilder.HasIndex(i => i.Phone);
        entityTypeBuilder.HasIndex(i => i.DateOfBirth);
        entityTypeBuilder.HasIndex(i => i.IntakeDate);
        entityTypeBuilder.HasIndex(i => i.Status);

        
        // relationships
            // Intake → Subject relationship (FIXED: Use Restrict to avoid cascade cycles)


            entityTypeBuilder
            .HasOne(i => i.Gender)
            .WithMany()
            .HasForeignKey(i => i.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

            entityTypeBuilder
            .HasOne(i => i.LeadSource)
            .WithMany(ls => ls.Intakes)
            .HasForeignKey(i => i.LeadSourceId)
            .OnDelete(DeleteBehavior.Restrict);

            //  intake -> branch , branch -> intakes

            entityTypeBuilder
            .HasOne(i => i.Branch)
            .WithMany()
            .HasForeignKey(i => i.BranchId) 
            .OnDelete(DeleteBehavior.Restrict);


             entityTypeBuilder
            .HasOne(i => i.Subject)
            .WithMany()
            .HasForeignKey(i => i.SubjectId) 
            .OnDelete(DeleteBehavior.Restrict);


             entityTypeBuilder
            .HasOne(i => i.CommercialAgent)
            .WithMany()
            .HasForeignKey(i => i.CommercialAgentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
