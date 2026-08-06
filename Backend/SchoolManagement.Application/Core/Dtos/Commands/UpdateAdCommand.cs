namespace SchoolManagement.Application.Core.Dtos.Commands;

public class UpdateAdCommand
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    public Guid BranchId { get; set; }
}
