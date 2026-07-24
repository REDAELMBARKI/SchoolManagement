using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Requests;

public class UpdatePaymentRequestDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public decimal? TransferFees { get; set; }

    public PaymentMethod Method { get; set; }

    public DateTime? PaidAt { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Guid BranchId { get; set; }
    public Guid ReceivedByStaffId { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
}