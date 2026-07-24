using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Responses;

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal FeeAmount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Foreign Keys
    public Guid EnrollmentId { get; set; }
    
    // Navigation Properties
    public EnrollmentResponseDto? Enrollment { get; set; }
}
