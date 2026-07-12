using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Backend.Dtos.Requests;

public class EnrollmentRequestDto
{
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
    
    [Required]
    public int GroupId { get; set; }
    
    [Required]
    public int BranchId { get; set; }
    
    [Required]
    public int PlanId { get; set; }
    
    public string? Notes { get; set; }
}
