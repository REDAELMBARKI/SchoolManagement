using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.PaidAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.WaivedAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.WaivedReason)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(c => c.InvoiceId)
            .IsUnique();

        builder.HasOne(c => c.Invoice)
            .WithOne(i => i.Charge)
            .HasForeignKey<Charge>(c => c.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
