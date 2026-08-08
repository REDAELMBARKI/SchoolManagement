namespace SchoolManagement.Application.Core.Dtos.Requests;

public class UpdateInvoiceRequestDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
}
