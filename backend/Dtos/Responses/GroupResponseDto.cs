namespace SchoolManagement.Backend.Dtos.Responses;

public class GroupResponseDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int Capacity { get; set; } = 15;
    
    public string Period { get; set; } = string.Empty; // Morning / Afternoon / Evening / Weekend
    
    // Foreign Keys
    public int BranchId { get; set; }
    public int LevelId { get; set; }
    public int LanguageId { get; set; }
    
    // Navigation Properties
    public BranchResponseDto? Branch { get; set; }
    public LevelResponseDto? Level { get; set; }
    public SubjectResponseDto? Subject { get; set; }
    
    public ICollection<StudentResponseDto> Students { get; set; } = new List<StudentResponseDto>();
    
    public ICollection<GroupTeacherResponseDto> Teachers { get; set; } = new List<GroupTeacherResponseDto>();
}
