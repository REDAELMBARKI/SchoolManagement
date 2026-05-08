namespace SchoolManagement.Backend.Dtos.Responses;

public class GroupResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; } = 15;
    public string Period { get; set; } = string.Empty; // Morning / Afternoon / Evening / Weekend
    public LevelResponseDto? Level { get; set; }
    public SubjectResponseDto? Subject { get; set; }
    public ICollection<GroupTeacherResponseDto> Teachers { get; set; } = new List<GroupTeacherResponseDto>();
}
