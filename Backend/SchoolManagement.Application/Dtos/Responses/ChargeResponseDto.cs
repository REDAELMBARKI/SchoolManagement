namespace SchoolManagement.Application.Dtos.Responses;

public class ChargeResponseDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }

    public Guid BranchId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}