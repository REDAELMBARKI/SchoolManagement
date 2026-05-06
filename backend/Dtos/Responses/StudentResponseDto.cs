using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Dtos.Responses;

public class StudentResponseDto
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string? Slug { get; set; }
    
    public string? Email { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    
    public DateOnly DateOfBirth { get; set; }
    
    // Foreign Keys
    public int? IntakeId { get; set; }

    // Navigation Properties
    public IntakeResponseDto? Intake { get; set; }

    public ICollection<Parent> Parents {get;set;} =  new List<Parent>() ; 
    public ParentResponseDto ParentResponseDto {get;set;} = null!;
    public ICollection<StudentParentResponseDto> StudentParents { get; set; } = new List<StudentParentResponseDto>();
    
    public ICollection<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
    
    // Inherited from Person
    public GenderResponseDto Gender { get; set; } = null!;
    
    // Avatar - single main media
    public MediaResponseDto? Avatar { get; set; }
}
