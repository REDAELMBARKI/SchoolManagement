namespace SchoolManagement.Backend.Dtos.Responses;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // Foreign Keys
    public int EnrollmentId { get; set; }
    
    // Navigation Properties
    public EnrollmentResponseDto? Enrollment { get; set; }
}
