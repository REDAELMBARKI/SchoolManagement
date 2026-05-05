using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

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
