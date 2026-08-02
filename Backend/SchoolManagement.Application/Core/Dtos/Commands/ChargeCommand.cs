namespace SchoolManagement.Application.Core.Dtos.Commands;

public class ChargeCommand
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}
