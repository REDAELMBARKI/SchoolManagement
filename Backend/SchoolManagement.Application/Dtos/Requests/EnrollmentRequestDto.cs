using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class EnrollmentRequestDto
{
    [Required]
    public Guid StudentId { get; set; }
    
    [Required]
    public Guid SubjectId { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    
    [Required]
    public Guid BranchId { get; set; }
    
    [Required]
    public Guid PlanId { get; set; }
    
    public string? Notes { get; set; }
}
