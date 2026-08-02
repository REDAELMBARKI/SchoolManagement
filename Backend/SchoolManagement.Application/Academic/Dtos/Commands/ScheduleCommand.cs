namespace SchoolManagement.Application.Academic.Dtos.Commands;

public class ScheduleCommand
{
    public Guid TeacherId { get; set; }
    public Guid RoomId { get; set; }
    public Guid DayId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid GroupId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid BranchId { get; set; }
}
