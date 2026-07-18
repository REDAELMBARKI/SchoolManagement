using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Dtos.Responses;

public class StudentResponseDto
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string? Slug { get; set; }
    
    public string? Email { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    
    public DateOnly DateOfBirth { get; set; }
    
    // Foreign Keys
    public Guid? IntakeId { get; set; }

    // Navigation Properties
    public IntakeResponseDto? Intake { get; set; }

    public IEnumerable<ParentResponseDto> Parents {get;set;} =  new List<ParentResponseDto>();
    
    public IEnumerable<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
    
    public GenderResponseDto Gender { get; set; } = null!;
    
    public MediaResponseDto? Avatar { get; set; }
}
