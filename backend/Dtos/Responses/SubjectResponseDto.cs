using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Backend.Dtos.Responses;

public class SubjectResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Slug { get; set; }
    
    public string? Description { get; set; }
    
    // Foreign Keys
    public int BranchId { get; set; }
    
    // Navigation Properties
    public BranchResponseDto? Branch { get; set; }
    
    public ICollection<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
}
