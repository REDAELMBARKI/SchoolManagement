namespace SchoolManagement.Application.Dtos.Responses;

public class LevelResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public ICollection<GroupResponseDto> Groups { get; set; } = new List<GroupResponseDto>();
}
