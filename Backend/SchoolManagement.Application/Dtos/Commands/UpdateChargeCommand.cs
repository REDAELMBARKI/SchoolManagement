using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Commands;

public class UpdateChargeCommand
{
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public Guid BranchId { get; set; }
    public Guid StudentId { get; set; }
    public ChargeType ChargeType { get; set; }
    public DateTime IssuedDate { get; set; }
    public decimal AmountPaid { get; set; }
    public Guid? SourceId { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}
