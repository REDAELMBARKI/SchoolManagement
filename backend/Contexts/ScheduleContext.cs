using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Contexts;

public class ScheduleContext : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasOne(sc => sc.Branch)
            .WithMany()
            .HasForeignKey(sc => sc.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.Teacher)
            .WithMany()
            .HasForeignKey(sc => sc.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.Subject)
            .WithMany()
            .HasForeignKey(sc => sc.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

            
        builder.HasOne(sc => sc.Room)
            .WithMany()
            .HasForeignKey(sc => sc.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.Group)
            .WithMany()
            .HasForeignKey(sc => sc.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.TimeSlot)
            .WithMany()
            .HasForeignKey(sc => sc.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.Day)
            .WithMany()
            .HasForeignKey(sc => sc.DayId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
