using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class GroupTeacherConfiguration : IEntityTypeConfiguration<GroupTeacher>
{
    public void Configure(EntityTypeBuilder<GroupTeacher> entityTypeBuilder)
    {
        // GroupTeacher → Teacher relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(gt => gt.Teacher)
            .WithMany()
            .HasForeignKey(gt => gt.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // GroupTeacher → Group relationship (FIXED: Use Restrict to avoid cascade cycles)
        entityTypeBuilder.HasOne(gt => gt.Group)
            .WithMany()
            .HasForeignKey(gt => gt.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
