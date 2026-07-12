namespace SchoolManagement.Backend.Dtos.Responses;

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
