using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class EnrollmentRequestDto
{

    [Required]
    public Guid PreferedScheduleId { get; set; }

    [Required]
    public Guid LevelId { get; set; }

    [Required]
    public Guid StudentId { get; set; }
    
    [Required]
    public Guid SubjectId { get; set; }
    
    [Required]
    public Guid PlanId { get; set; }
    
    public string? Notes { get; set; }

    // can be computed or provided from the form 
    public Guid? GroupId { get; set; }

}
