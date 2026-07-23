namespace SchoolManagement.Application.Dtos.Responses;

public class StudentRegistrationResponseDto
{
    public StudentResponseDto StudentRegRes { get; set; } = null!;
    public EnrollmentResponseDto EnrollmentRegRes { get; set; } = null!;
    public ChargeResponseDto? ChargeRegRes { get; set; }
    public PaymentResponseDto? PaymentRegRes { get; set; }
}
