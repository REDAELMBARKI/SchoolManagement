using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class ScheduleRequestDto
{
    [Required]
    public Guid BranchId { get; set; }
    
    [Required]
    public Guid TeacherId { get; set; }
    
    [Required]
    public Guid RoomId { get; set; }
    
    [Required]
    public Guid DayId { get; set; }
    
    [Required]
    public Guid TimeSlotId { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    
    [Required]
    public Guid SubjectId { get; set; }
}
