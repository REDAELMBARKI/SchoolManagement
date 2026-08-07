using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class RecordInvoicePaymentRequestDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    [Required]
    public DateTime PaidAt { get; set; }

    public decimal? TransferFees { get; set; }

    public string? ExternalReferenceCode { get; set; }

    public string MethodDetailsJson { get; set; } = "{}";
}
