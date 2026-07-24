namespace SchoolManagement.Application.Dtos.Responses;

public class PlanResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty; // "1 Month", "3 Months", "Full Year"
    
    public int DurationMonths { get; set; }            // 1, 3, 6, 12
    
    public decimal BaseAmount { get; set; }            // the actual fee before discount
    
    public decimal? DiscountPercent { get; set; }
    
    public bool IsActive { get; set; } = true;
    public int RemainingAmountDueDate { get; set; }
    
    public ICollection<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
}
