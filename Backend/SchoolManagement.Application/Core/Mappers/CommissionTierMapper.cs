using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class CommissionTierMapper
{
    public static CommissionTier ToDomain(CommissionTierCommand command)
    {
        return CommissionTier.Create(
            minSalesCount: command.MinSalesCount,
            maxSalesCount: command.MaxSalesCount,
            amount: command.Amount,
            displayOrder: command.DisplayOrder);
    }

    public static CommissionTierResponseDto ToResponse(CommissionTier tier)
    {
        return new CommissionTierResponseDto
        {
            Id = tier.Id,
            MinSalesCount = tier.MinSalesCount,
            MaxSalesCount = tier.MaxSalesCount,
            Amount = tier.Amount,
            IsActive = tier.IsActive,
            DisplayOrder = tier.DisplayOrder,
            CreatedAt = tier.CreatedAt
        };
    }
}
