namespace SchoolManagement.Backend.Dtos.Responses;

public class PlanResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty; // "1 Month", "3 Months", "Full Year"
    
    public int DurationMonths { get; set; }            // 1, 3, 6, 12
    
    public decimal? DiscountPercent { get; set; }
    
    public ICollection<EnrollmentResponseDto> Enrollments { get; set; } = new List<EnrollmentResponseDto>();
}
