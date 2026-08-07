using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Commands;

/// <summary>
/// Command for recording payment - includes InvoiceId from route
/// </summary>
public class RecordInvoicePaymentCommand
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; }
    public decimal? TransferFees { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string MethodDetailsJson { get; set; } = "{}";
    public Guid ReceivedByStaffId { get; set; }
    public Guid BranchId { get; set; }
}
