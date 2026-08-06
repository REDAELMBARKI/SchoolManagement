namespace SchoolManagement.Application.Core.Dtos.Commands;

public class AdCommand
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    
    // Populated by service from current user context
    public Guid BranchId { get; set; }
}
