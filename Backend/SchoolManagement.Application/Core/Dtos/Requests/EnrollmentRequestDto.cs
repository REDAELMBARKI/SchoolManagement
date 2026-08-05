using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class EnrollmentRequestDto 
{


    [Required]
    public Guid LevelId { get; set; }


    [Required]
    public Guid StudentId { get; set; }

    [Required]

    public Guid SubjectId { get; set; }

    [Required]

    public Guid PlanId { get; set; }
    
    public string? Notes { get; set; }

    public Guid? PreferedGroupId { get; set; }

}
