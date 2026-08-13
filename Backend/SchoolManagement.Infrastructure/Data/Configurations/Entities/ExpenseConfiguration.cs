using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(30)
            .HasConversion<string>();

        builder.Property(e => e.PayeeName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.ExpenseDate)
            .IsRequired();

        builder.Property(e => e.PaymentMethod)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(e => e.Reference)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(e => e.ProcessedByStaffId)
            .IsRequired();

        builder.Property(e => e.BranchId)
            .IsRequired();

        builder.Property(e => e.CurrencyCode)
            .IsRequired()
            .HasMaxLength(10);

        // Relationship: many expenses → one branch
        builder.HasOne(e => e.Branch)
            .WithMany()
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(e => e.BranchId);
        builder.HasIndex(e => e.ExpenseDate);
        builder.HasIndex(e => e.ProcessedByStaffId);
    }
}
