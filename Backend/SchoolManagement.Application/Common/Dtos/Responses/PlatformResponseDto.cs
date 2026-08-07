namespace SchoolManagement.Application.Common.Dtos.Responses;

public class PlatformResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
