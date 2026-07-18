namespace SchoolManagement.Application.Dtos.Responses;

public class TimeSlotResponseDto
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
