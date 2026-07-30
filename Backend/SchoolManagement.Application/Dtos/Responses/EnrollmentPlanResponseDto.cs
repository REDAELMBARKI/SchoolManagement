namespace SchoolManagement.Application.Dtos.Responses;

public class EnrollmentPlanResponseDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime CreatedAt { get; set; }
    public PlanResponseDto? Plan { get; set; }
}
