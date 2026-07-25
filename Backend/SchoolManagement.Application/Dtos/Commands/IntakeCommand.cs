using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Commands;

public class IntakeCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Email { get; set; } = null!;
    public string? Phone { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Guid? GenderId { get; set; }
    public DateTime IntakeDate { get; set; }
    public IntakeStatus Status { get; set; } = IntakeStatus.New;
    public DateTime? FollowUpDate { get; set; }
    public string? Notes { get; set; }
    public Guid? CommercialAgentId { get; set; }
    public Guid? LeadSourceId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid BranchId { get; set; }
    public bool IsIndependent { get; set; } = false;
    public decimal TotalFees { get; set; }
    public decimal AmountPaid { get; set; }
}
