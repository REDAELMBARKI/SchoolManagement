using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class ChargeSettlementPaymentRequestDto
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public Guid InvoiceId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}
