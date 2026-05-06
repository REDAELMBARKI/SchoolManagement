using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> entityTypeBuilder)
    {
        entityTypeBuilder.Property(s => s.DayOfWeek)
            .IsRequired()
            .HasMaxLength(15);

        entityTypeBuilder.Property(s => s.Color)
            .HasMaxLength(10);

        entityTypeBuilder.Property(s => s.BranchId)
            .IsRequired();

        entityTypeBuilder.Property(s => s.GroupTeacherId)
            .IsRequired();

        entityTypeBuilder.Property(s => s.RoomId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(s => s.BranchId);
        entityTypeBuilder.HasIndex(s => s.GroupTeacherId);
        entityTypeBuilder.HasIndex(s => s.RoomId);

        // Schedule → Branch relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule → GroupTeacher relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(s => s.GroupTeacher)
            .WithMany()
            .HasForeignKey(s => s.GroupTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule → Room relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(s => s.Room)
            .WithMany()
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
