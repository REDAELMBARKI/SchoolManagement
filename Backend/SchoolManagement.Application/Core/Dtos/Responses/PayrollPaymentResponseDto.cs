using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Responses;

public class PayrollPaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetAmount { get; set; }
    public int PayPeriodMonth { get; set; }
    public int PayPeriodYear { get; set; }
    public PayrollStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? ReferenceCode { get; set; }
    public Guid BranchId { get; set; }
    public Guid ProcessedByStaffId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
