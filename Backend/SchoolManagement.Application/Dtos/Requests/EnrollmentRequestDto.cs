using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class EnrollmentRequestDto
{


    [Required]
    public Guid LevelId { get; set; }

    [Required]

    public Guid SubjectId { get; set; }

    [Required]

    public Guid PlanId { get; set; }
    
    public string? Notes { get; set; }

    public Guid BranchId { get; set; }
    public Guid? PreferedScheduleId { get; set; }
    public Guid? GroupId { get; set; }

}
