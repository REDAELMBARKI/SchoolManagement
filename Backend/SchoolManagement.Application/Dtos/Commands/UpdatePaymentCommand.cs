using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Commands;

public class UpdatePaymentCommand
{
    public Guid EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public Guid BranchId { get; set; }
    public Guid ReceivedByStaffId { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string MethodDetailsJson { get; set; } = "{}";
}
