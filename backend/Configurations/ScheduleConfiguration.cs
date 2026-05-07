using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> entityTypeBuilder)
    {
        entityTypeBuilder.Property(s => s.DayId)
            .IsRequired()
            .HasMaxLength(15);

        entityTypeBuilder.Property(s => s.TeacherId)
            .HasMaxLength(10);

        entityTypeBuilder.Property(s => s.BranchId)
            .IsRequired();

            

        entityTypeBuilder.Property(s => s.GroupId)
            .IsRequired();

        entityTypeBuilder.Property(s => s.RoomId)
            .IsRequired();

        // Indexes for performance
        entityTypeBuilder.HasIndex(s => s.BranchId);
        entityTypeBuilder.HasIndex(s => s.RoomId);

       
    }
}
