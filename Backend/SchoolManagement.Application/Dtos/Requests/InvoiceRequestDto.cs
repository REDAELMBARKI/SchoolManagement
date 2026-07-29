namespace SchoolManagement.Application.Dtos.Requests;

public class InvoiceRequestDto
{
    public Guid EnrollmentId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public List<ChargeRequestDto> Charges { get; set; } = new();
}

public class ChargeRequestDto
{
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
}
