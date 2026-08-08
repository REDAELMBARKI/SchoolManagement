namespace SchoolManagement.Application.Core.Dtos.Requests;

public class CreateInvoiceRequestDto
{
    public Guid EnrollmentId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public ChargeCommand? Charge { get; set; }
}
