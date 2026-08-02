namespace SchoolManagement.Application.Core.Dtos.Commands;

public class WaiveInvoiceCommand
{
    public Guid InvoiceId { get; set; }
    public decimal WaivedAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid? WaivedByUserId { get; set; }
}
