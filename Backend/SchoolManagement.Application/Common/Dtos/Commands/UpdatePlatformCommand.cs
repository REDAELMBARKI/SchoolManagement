namespace SchoolManagement.Application.Common.Dtos.Commands;

public record UpdatePlatformCommand
{
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
}
