using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class IntakeConfiguration : IEntityTypeConfiguration<Intake>
{
    public void Configure(EntityTypeBuilder<Intake> entityTypeBuilder)
    {
        // TPC mapping for Intake entity
        entityTypeBuilder.ToTable("Intakes");

        // FirstName validation
        entityTypeBuilder.Property(i => i.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        // LastName validation
        entityTypeBuilder.Property(i => i.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // Email validation
        entityTypeBuilder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(255);

        // Phone validation
        entityTypeBuilder.Property(i => i.Phone)
            .IsRequired()
            .HasMaxLength(20);

        // Slug validation
        entityTypeBuilder.Property(i => i.Slug)
            .IsRequired()
            .HasMaxLength(100);

        // IntakeDate validation
        entityTypeBuilder.Property(i => i.IntakeDate)
            .IsRequired();

        // DateOfBirth validation
        entityTypeBuilder.Property(i => i.DateOfBirth)
            .IsRequired(false);

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

        entityTypeBuilder.Property(i => i.SchoolProgramId)
            .IsRequired();

        entityTypeBuilder.Property(i => i.BranchId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(i => i.Email).IsUnique();
        entityTypeBuilder.HasIndex(i => i.Phone);
        entityTypeBuilder.HasIndex(i => i.Slug);
        entityTypeBuilder.HasIndex(i => i.IntakeDate);
        entityTypeBuilder.HasIndex(i => i.Status);

        // Check constraints for business rules
        entityTypeBuilder.ToTable("Intakes", tb =>
        {
            tb.HasCheckConstraint("CK_Intake_IntakeDate", "IntakeDate <= GETDATE()");
            tb.HasCheckConstraint("CK_Intake_DateOfBirth", "DateOfBirth IS NULL OR DateOfBirth < GETDATE()");
            tb.HasCheckConstraint("CK_Intake_FollowUpDate", "FollowUpDate IS NULL OR FollowUpDate >= IntakeDate");
        });
    }
}
