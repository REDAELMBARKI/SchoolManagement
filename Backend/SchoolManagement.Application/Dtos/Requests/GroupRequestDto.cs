namespace SchoolManagement.Application.Dtos.Requests;

public class GroupRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; } = 15;
    public string Period { get; set; } = string.Empty; // Morning / Afternoon / Evening / Weekend
    public Guid LevelId { get; set; }
    public Guid SubjectId { get; set; }
}
