namespace SchoolManagement.Application.Dtos.Responses;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal FeeAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? PaidAt { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Foreign Keys
    public int EnrollmentId { get; set; }
    
    // Navigation Properties
    public EnrollmentResponseDto? Enrollment { get; set; }
}
