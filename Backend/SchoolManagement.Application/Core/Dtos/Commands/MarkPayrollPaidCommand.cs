using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class MarkPayrollPaidCommand
{
    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(100)]
    public string? ReferenceCode { get; set; }
}
