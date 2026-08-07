namespace SchoolManagement.Application.Academic.Dtos.Responses;

public class LevelResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
