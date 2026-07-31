namespace SchoolManagement.Application.Academic.Dtos.Commands;

public class UpdateGroupCommand
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; } = 15;
    public string Period { get; set; } = string.Empty;
    public Guid LevelId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid BranchId { get; set; }
}
