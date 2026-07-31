namespace SchoolManagement.Application.Core.Dtos.Commands;

public class InvoiceCommand
{
    public Guid EnrollmentId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public Guid BranchId { get; set; }
    public ChargeCommand? Charge { get; set; }
}
