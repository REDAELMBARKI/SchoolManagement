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

        // Intake → Subject relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(i => i.Subject)
            .WithMany()
            .HasForeignKey(i => i.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

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

        entityTypeBuilder.Property(i => i.ConvertedToStudentId)
            .IsRequired(false);

        // Indexes for performance
        entityTypeBuilder.HasIndex(i => i.Email).IsUnique();
        entityTypeBuilder.HasIndex(i => i.Phone);
        entityTypeBuilder.HasIndex(i => i.DateOfBirth);
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
