namespace SchoolManagement.Application.Core.Dtos.Requests;

public class InvoiceRequestDto
{
    public Guid EnrollmentId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public ChargeRequestDto? Charge { get; set; }
}

public class ChargeRequestDto
{
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
}
