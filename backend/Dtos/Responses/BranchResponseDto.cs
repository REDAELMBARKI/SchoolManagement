namespace SchoolManagement.Backend.Dtos.Responses;

public class BranchResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Slug { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
    
    public ICollection<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
}
