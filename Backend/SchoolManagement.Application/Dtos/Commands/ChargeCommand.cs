namespace SchoolManagement.Application.Dtos.Commands;

public class ChargeCommand
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}
