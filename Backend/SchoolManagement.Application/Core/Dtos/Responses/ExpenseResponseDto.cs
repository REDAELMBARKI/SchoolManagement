namespace SchoolManagement.Application.Core.Dtos.Responses;

public class ExpenseResponseDto
{
    public Guid Id { get; set; }
    public ExpenseType Category { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public Guid ProcessedByStaffId { get; set; }
    public Guid BranchId { get; set; }
    public string CurrencyCode { get; set; } = "MAD";
    public DateTime CreatedAt { get; set; }
}
