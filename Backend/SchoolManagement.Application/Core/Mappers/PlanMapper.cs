using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class PlanMapper
{
    public static Plan ToDomain(PlanCommand command, Guid branchId)
    {
        return Plan.Create(
            name: command.Name,
            durationMonths: command.DurationMonths,
            remainingAmountDueDays: command.RemainingAmountDueDays,
            baseAmount: command.BaseAmount,
            branchId: branchId,
            discountPercent: command.DiscountPercent,
            isActive: command.IsActive
        );
    }

    public static PlanResponseDto ToResponse(Plan plan)
    {
        return new PlanResponseDto
        {
            Id = plan.Id,
            Name = plan.Name,
            DurationMonths = plan.DurationMonths,
            BaseAmount = plan.BaseAmount,
            DiscountPercent = plan.DiscountPercent,
            Amount = plan.Amount,
            IsActive = plan.IsActive,
            RemainingAmountDueDays = plan.RemainingAmountDueDays,
            BranchId = plan.BranchId,
            CreatedAt = plan.CreatedAt
        };
    }
}
