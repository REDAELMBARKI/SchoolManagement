using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Responses;

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime PaidAt { get; set; }
    public Guid BranchId { get; set; }
    public Guid ReceivedByStaffId { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string MethodDetailsJson { get; set; } = "{}";
    public string CurrencyCode { get; set; } = "USD";
}
