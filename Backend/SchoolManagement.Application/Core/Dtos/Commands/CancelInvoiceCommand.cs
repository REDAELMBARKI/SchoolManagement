namespace SchoolManagement.Application.Core.Dtos.Commands;

public class CancelInvoiceCommand
{
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid CancelledByUserId { get; set; }
}
