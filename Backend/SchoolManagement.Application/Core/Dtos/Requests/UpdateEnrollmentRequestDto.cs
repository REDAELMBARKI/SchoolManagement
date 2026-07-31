using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class UpdateEnrollmentRequestDto
{

    [Required] 
    public Guid Id { get; set; }

    [Required]
    public Guid PreferedScheduleId { get; set; }

    [Required]
    public Guid LevelId { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    public string? Notes { get; set; }

    public Guid GroupId { get; set; }

    public Guid? PlanId { get; set; }
}
