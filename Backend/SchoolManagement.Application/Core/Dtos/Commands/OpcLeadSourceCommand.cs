namespace SchoolManagement.Application.Core.Dtos.Commands;

public class OpcLeadSourceCommand
{
    public Guid OpcId { get; set; }
    
    // Populated by service from current user context
    public Guid BranchId { get; set; }
}
