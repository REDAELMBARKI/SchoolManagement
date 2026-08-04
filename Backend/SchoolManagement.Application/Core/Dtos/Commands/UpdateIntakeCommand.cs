using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class UpdateIntakeCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
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
    public bool IsIndependent { get; set; }
    public decimal TotalFees { get; set; }
    public decimal AmountPaid { get; set; }
}
