using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class EnrollmentPlanConfiguration : IEntityTypeConfiguration<EnrollmentPlan>
{
    public void Configure(EntityTypeBuilder<EnrollmentPlan> builder)
    {
        builder.HasKey(ep => ep.Id);

        builder.Property(ep => ep.EnrollmentId)
            .IsRequired();

        builder.Property(ep => ep.PlanId)
            .IsRequired();

        builder.Property(ep => ep.CreatedAt)
            .IsRequired();

        builder.HasOne(ep => ep.Enrollment)
            .WithMany(e => e.EnrollmentPlans)
            .HasForeignKey(ep => ep.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ep => ep.Plan)
            .WithMany()
            .HasForeignKey(ep => ep.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ep => ep.EnrollmentId);
        builder.HasIndex(ep => ep.PlanId);
    }
}
