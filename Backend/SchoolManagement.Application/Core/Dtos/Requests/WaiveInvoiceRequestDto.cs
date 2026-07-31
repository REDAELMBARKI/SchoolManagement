namespace SchoolManagement.Application.Core.Dtos.Requests;

public class WaiveInvoiceRequestDto
{
    public decimal WaivedAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
}
