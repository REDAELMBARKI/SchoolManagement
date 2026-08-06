namespace SchoolManagement.Application.Core.Dtos.Commands;

public class AdLeadSourceCommand
{
    public Guid AdId { get; set; }
    
    // Populated by service from current user context
    public Guid BranchId { get; set; }
}
