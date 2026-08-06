using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Infrastructure.Common.Configurations;

public class WhatsAppMessageConfiguration : IEntityTypeConfiguration<WhatsAppMessage>
{
    public void Configure(EntityTypeBuilder<WhatsAppMessage> builder)
    {
        builder.ToTable("WhatsAppMessages");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Message)
            .IsRequired()
            .HasMaxLength(4096); // WhatsApp message limit

        builder.Property(w => w.MessageType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.EntityType)
            .HasMaxLength(100);

        builder.Property(w => w.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(w => w.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(w => w.Branch)
            .WithMany()
            .HasForeignKey(w => w.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => new { w.EntityType, w.EntityId });
        builder.HasIndex(w => w.CreatedAt);
    }
}
