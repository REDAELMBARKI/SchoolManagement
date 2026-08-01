using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class PayrollPaymentConfiguration : IEntityTypeConfiguration<PayrollPayment>
{
    public void Configure(EntityTypeBuilder<PayrollPayment> builder)
    {
        builder.ToTable("PayrollPayments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.EmployeeId)
            .IsRequired();

        builder.Property(p => p.GrossAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Bonus)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Deductions)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.NetAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.PayPeriodMonth)
            .IsRequired();

        builder.Property(p => p.PayPeriodYear)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(p => p.PaidAt)
            .IsRequired(false);

        builder.Property(p => p.PaymentMethod)
            .IsRequired(false)
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(p => p.ReferenceCode)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(p => p.BranchId)
            .IsRequired();

        builder.Property(p => p.ProcessedByStaffId)
            .IsRequired();

        builder.Property(p => p.Notes)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(p => p.CurrencyCode)
            .IsRequired()
            .HasMaxLength(10);

        // Relationship: many payroll payments → one branch
        builder.HasOne(p => p.Branch)
            .WithMany()
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.EmployeeId);
        builder.HasIndex(p => p.BranchId);
        builder.HasIndex(p => new { p.PayPeriodMonth, p.PayPeriodYear });
        builder.HasIndex(p => new { p.EmployeeId, p.PayPeriodMonth, p.PayPeriodYear });
    }
}
