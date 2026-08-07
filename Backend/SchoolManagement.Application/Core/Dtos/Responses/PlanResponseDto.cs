namespace SchoolManagement.Application.Core.Dtos.Responses;

public class PlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal Amount { get; set; } // Calculated: BaseAmount - discount
    public bool IsActive { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RemainingAmountDueDays { get; set; }
}
