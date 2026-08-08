namespace SchoolManagement.Application.Core.Dtos.Requests;

public record CreateCommissionTierRequestDto
{
    public int MinSalesCount { get; init; }
    public int? MaxSalesCount { get; init; }
    public decimal Amount { get; init; }
    public int DisplayOrder { get; init; }
}
