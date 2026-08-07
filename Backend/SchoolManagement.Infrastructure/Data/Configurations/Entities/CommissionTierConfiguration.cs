using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class CommissionTierConfiguration : IEntityTypeConfiguration<CommissionTier>
{
    public void Configure(EntityTypeBuilder<CommissionTier> builder)
    {
        builder.ToTable("CommissionTiers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MinSalesCount)
            .IsRequired();

        builder.Property(t => t.MaxSalesCount)
            .IsRequired(false);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(t => t.IsActive);
        builder.HasIndex(t => t.DisplayOrder);
        builder.HasIndex(t => new { t.IsActive, t.DisplayOrder });
    }
}
