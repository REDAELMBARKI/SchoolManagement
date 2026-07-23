using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class StudentRegistrationRequestDto
{
    [Required]
    public StudentRequestDto StudentReqReg { get; set; } = null!;
    
    [Required]
    public EnrollmentRequestDto EnrollmentReqReg { get; set; } = null!;

    public ChargeRequestDto? ChargeReq { get; set; }

    public PaymentRequestDto? PaymentReq { get; set; }
}

