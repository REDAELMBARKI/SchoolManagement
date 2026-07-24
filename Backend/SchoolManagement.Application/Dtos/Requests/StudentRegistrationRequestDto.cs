using SchoolManagement.Application.Dtos.Responses;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class StudentRegistrationRequestDto
{
    [Required]
    public StudentRequestDto StudentRegReq { get; set; } = null!;
    
    [Required]
    public EnrollmentRequestDto EnrollmentRegReq { get; set; } = null!;

    [Required]
    public PaymentRequestDto PaymentRegReq { get; set; } = null!;
}

