namespace SchoolManagement.Application.Core.Dtos.Responses;

public class AdResponseDto
{
    public Guid Id { get; set; }
    public LeadSourceType Type { get; set; } = LeadSourceType.Ad;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
