namespace SchoolManagement.Application.Core.Dtos.Commands;

public class ExpenseCommand
{
    public ExpenseType Category { get; set; }
    public string PayeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }

    // Populated by the service from the current user context
    public Guid BranchId { get; set; }
    public Guid ProcessedByStaffId { get; set; }
}
