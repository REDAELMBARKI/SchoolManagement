using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Requests;

public class PaymentRequestDto
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public decimal FeeAmount { get; set; }

    [Required]
    public DateTime PeriodStart { get; set; }

    [Required]
    public DateTime PeriodEnd { get; set; }

    public decimal AmountPaid { get; set; } = 0;

    public DateTime? PaidAt { get; set; }

    public string Status { get; set; } = "Pending";
}