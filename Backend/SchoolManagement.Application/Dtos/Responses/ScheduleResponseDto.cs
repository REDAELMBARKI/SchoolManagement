namespace SchoolManagement.Application.Dtos.Responses;

public class ScheduleResponseDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid TeacherId { get; set; }
    public Guid RoomId { get; set; }
    public Guid DayId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid GroupId { get; set; }
    public Guid SubjectId { get; set; }
    
    public BranchResponseDto? Branch { get; set; }
    public TeacherResponseDto? Teacher { get; set; }
    public RoomResponseDto? Room { get; set; }
    public DayResponseDto? Day { get; set; }
    public TimeSlotResponseDto? TimeSlot { get; set; }
    public GroupResponseDto? Group { get; set; }
    public SubjectResponseDto? Subject { get; set; }
}

public class GroupedScheduleDto
{
    public List<DayScheduleDto> Days { get; set; } = new List<DayScheduleDto>();
}

public class DayScheduleDto
{
    public string DayName { get; set; } = string.Empty;
    public List<TimeSlotScheduleDto> TimeSlots { get; set; } = new List<TimeSlotScheduleDto>();
}

public class TimeSlotScheduleDto
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherFullName { get; set; } = string.Empty;
}
