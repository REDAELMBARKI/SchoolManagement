using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class RegistrationPaymentRequestDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal AmountPaid { get; set; }
    public decimal? TransferFees { get; set; }
    public DateTime? PaidAt { get; set; } = DateTime.UtcNow;
    public PaymentMethod Method { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
}
