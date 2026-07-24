using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Requests;

public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
}