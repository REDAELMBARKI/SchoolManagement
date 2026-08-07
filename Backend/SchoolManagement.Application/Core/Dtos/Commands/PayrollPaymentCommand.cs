using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class PayrollPaymentCommand
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal GrossAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Bonus { get; set; } = 0;

    [Range(0, double.MaxValue)]
    public decimal Deductions { get; set; } = 0;

    [Range(1, 12)]
    public int PayPeriodMonth { get; set; }

    [Range(2000, 2100)]
    public int PayPeriodYear { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
