using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
{
    public void Configure(EntityTypeBuilder<Commission> builder)
    {
        builder.ToTable("Commissions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.EarnerId)
            .IsRequired();

        builder.Property(c => c.EarnerType)
            .IsRequired()
            .HasMaxLength(30)
            .HasConversion<string>();

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.PeriodMonth)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        // FK to CommissionTier (optional, required for Agent, null for OPC)
        builder.Property(c => c.CommissionTierId)
            .IsRequired(false);

        builder.HasOne(c => c.CommissionTier)
            .WithMany()
            .HasForeignKey(c => c.CommissionTierId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent tier deletion if commissions reference it

        // OPC fields
        builder.Property(c => c.SourceEnrollmentId)
            .IsRequired(false);

        // Agent fields
        builder.Property(c => c.SalesCountAtCalculation)
            .IsRequired(false);

        builder.Property(c => c.BlockReason)
            .HasMaxLength(500)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(c => c.EarnerId);
        builder.HasIndex(c => c.PeriodMonth);
        builder.HasIndex(c => new { c.EarnerId, c.PeriodMonth });
        builder.HasIndex(c => c.SourceEnrollmentId);
        builder.HasIndex(c => c.CommissionTierId); // Index for FK lookups
    }
}
