using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Responses;

public class ChargeResponseDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public ChargeType ChargeType { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public ChargeStatus Status { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public Guid? SourceId { get; set; }
    public Guid BranchId { get; set; }
    public StudentResponseDto? Student { get; set; }
    public ICollection<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
}