namespace SchoolManagement.Application.Common.Dtos.Commands;

public record PlatformCommand
{
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public Guid BranchId { get; init; }
}
