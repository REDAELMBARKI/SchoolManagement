namespace SchoolManagement.Application.Dtos.Commands;

public class UpdateEnrollmentCommand
{
    public Guid PreferedScheduleId { get; set; }
    public Guid LevelId { get; set; }
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid PlanId { get; set; }
    public string? Notes { get; set; }
    public Guid GroupId { get; set; }
    public Guid BranchId { get; set; }
}
