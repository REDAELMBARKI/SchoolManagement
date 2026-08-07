namespace SchoolManagement.Application.Core.Dtos.Commands;

public record CommissionTierCommand
{
    public int MinSalesCount { get; init; }
    public int? MaxSalesCount { get; init; }
    public decimal Amount { get; init; }
    public int DisplayOrder { get; init; }
}
