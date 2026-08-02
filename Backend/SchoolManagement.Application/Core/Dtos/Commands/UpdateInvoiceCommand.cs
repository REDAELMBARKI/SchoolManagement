namespace SchoolManagement.Application.Core.Dtos.Commands;

public class UpdateInvoiceCommand
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
}
