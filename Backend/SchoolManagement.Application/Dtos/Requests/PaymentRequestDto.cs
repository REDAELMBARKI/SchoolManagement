using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Requests;

public class PaymentRequestDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}