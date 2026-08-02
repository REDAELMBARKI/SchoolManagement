using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class ChargeSettlementPaymentCommand
{
    public Guid EnrollmentId { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public Guid BranchId { get; set; }
    public Guid ReceivedByStaffId { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string MethodDetailsJson { get; set; } = "{}";
    public string CurrencyCode { get; set; } = "USD";
}
