namespace SchoolManagement.Application.Core.Dtos.Responses;

public class RefundResponseDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime RefundedAt { get; set; }
    public Guid RefundedByStaffId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
