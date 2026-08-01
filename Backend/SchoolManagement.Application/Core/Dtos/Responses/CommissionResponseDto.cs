using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Responses;

public class CommissionResponseDto
{
    public Guid Id { get; set; }
    public Guid EarnerId { get; set; }
    public EarnerType EarnerType { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PeriodMonth { get; set; }
    public CommissionStatus Status { get; set; }

    // OPC only
    public Guid? SourceEnrollmentId { get; set; }

    // Agent only
    public int? SalesCountAtCalculation { get; set; }
    public int? AppliedTierMin { get; set; }
    public int? AppliedTierMax { get; set; }

    public DateTime CreatedAt { get; set; }
}
