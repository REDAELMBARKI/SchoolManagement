namespace SchoolManagement.Application.Dtos.Responses;

public class StudentRegistrationResponseDto
{
    public StudentResponseDto Student { get; set; } = null!;
    public EnrollmentResponseDto Enrollment { get; set; } = null!;
}
