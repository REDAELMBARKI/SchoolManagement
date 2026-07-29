using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class StudentRegistrationRequestDto
{
    [Required]
    public StudentRequestDto StudentRegReq { get; set; } = null!;
    
    [Required]
    public EnrollmentRequestDto EnrollmentRegReq { get; set; } = null!;

    [Required]
    public RegistrationPaymentRequestDto PaymentRegReq { get; set; } = null!;

    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public DateTime? InvoiceDueDate { get; set; }
    public DateTime? ChargeDueDate { get; set; }
}
