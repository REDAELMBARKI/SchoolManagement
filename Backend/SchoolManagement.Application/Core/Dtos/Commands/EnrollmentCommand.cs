using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class EnrollmentCommand
{
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public string? Notes { get; set; }

    public Guid StudentId { get; set; }

    public Guid SubjectId { get; set; }

    public Guid GroupId { get; set; }

    public Guid BranchId { get; set; }

    public Guid LevelId { get; set; }

    public Guid? PlanId { get; set; }

    public Guid? PreferedScheduleId { get; set; }
}
