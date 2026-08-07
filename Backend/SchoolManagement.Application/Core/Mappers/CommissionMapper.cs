using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class CommissionMapper
{
    public static CommissionResponseDto ToResponse(Commission commission)
    {
        return new CommissionResponseDto
        {
            Id = commission.Id,
            EarnerId = commission.EarnerId,
            EarnerType = commission.EarnerType,
            Amount = commission.Amount,
            PeriodMonth = commission.PeriodMonth,
            Status = commission.Status,
            CommissionTierId = commission.CommissionTierId,
            SourceEnrollmentId = commission.SourceEnrollmentId,
            SalesCountAtCalculation = commission.SalesCountAtCalculation,
            CreatedAt = commission.CreatedAt
        };
    }
}
