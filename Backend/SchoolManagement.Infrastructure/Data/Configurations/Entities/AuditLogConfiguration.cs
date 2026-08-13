using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Branch)
            .WithMany()
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(a => a.BranchId);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.ChangedAt);
        builder.HasIndex(a => new { a.EntityName, a.EntityId });
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => a.Severity);
        builder.HasIndex(a => a.Category);

        // Property configurations
        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Message)
            .HasMaxLength(500);

        builder.Property(a => a.ChangedBy)
            .HasMaxLength(450);

        builder.Property(a => a.HasRole)
            .HasMaxLength(50);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45); 

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Severity)
            .HasMaxLength(20);

        builder.Property(a => a.Category)
            .HasMaxLength(50);

        builder.Property(a => a.ChangedAt)
            .IsRequired();
    }
}
