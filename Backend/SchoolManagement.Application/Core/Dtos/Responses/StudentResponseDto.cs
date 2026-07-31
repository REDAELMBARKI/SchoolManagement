using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Core.Dtos.Responses;

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
    public bool IsDirectRegistration { get; set; }
    public Guid BranchId { get; set; }

    // Navigation Properties
    public IntakeResponseDto? Intake { get; set; }

    public IEnumerable<StudentResponsableResponseDto> StudentResponsables {get;set;} =  new List<StudentResponsableResponseDto>();
    
    public IEnumerable<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
    
    public GenderResponseDto Gender { get; set; } = null!;
    
    public MediaResponseDto? Avatar { get; set; }

    public BranchResponseDto? Branch { get; set; }
}
