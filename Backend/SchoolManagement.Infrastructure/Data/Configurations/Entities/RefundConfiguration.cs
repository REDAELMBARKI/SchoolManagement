using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.PaymentId)
            .IsRequired();

        builder.Property(r => r.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.RefundedAt)
            .IsRequired();

        builder.Property(r => r.RefundedByStaffId)
            .IsRequired();

        builder.Property(r => r.BranchId)
            .IsRequired();

        // Relationship: many refunds → one payment
        builder.HasOne(r => r.Payment)
            .WithMany(p => p.Refunds)
            .HasForeignKey(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.PaymentId);
        builder.HasIndex(r => r.BranchId);
    }
}
