namespace SchoolManagement.Application.Core.Dtos.Responses;

public class ChargeResponseDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }

    public Guid BranchId { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal WaivedAmount { get; set; }
    public string? WaivedReason { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}