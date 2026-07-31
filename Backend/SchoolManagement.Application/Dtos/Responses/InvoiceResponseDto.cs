using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Responses;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal CreditAppliedAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public Guid BranchId { get; set; }
    public ChargeResponseDto? Charge { get; set; }
}
