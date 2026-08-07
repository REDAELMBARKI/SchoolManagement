namespace SchoolManagement.Application.Academic.Dtos.Requests;

public class CheckStudentScheduleConflictRequestDto
{
    public Guid StudentId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? ExcludeEnrollmentId { get; set; } // For update scenarios
}
