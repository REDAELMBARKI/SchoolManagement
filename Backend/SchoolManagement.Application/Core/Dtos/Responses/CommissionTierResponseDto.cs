namespace SchoolManagement.Application.Core.Dtos.Responses;

public class CommissionTierResponseDto
{
    public Guid Id { get; set; }
    public int MinSalesCount { get; set; }
    public int? MaxSalesCount { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
