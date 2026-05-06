namespace SchoolManagement.Backend.Dtos.Responses;

public class LevelResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int BranchId { get; set; }
    
    // Navigation Properties
    public BranchResponseDto? Branch { get; set; }
    
    public ICollection<GroupResponseDto> Groups { get; set; } = new List<GroupResponseDto>();
}
