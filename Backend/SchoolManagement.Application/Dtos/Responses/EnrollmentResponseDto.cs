namespace SchoolManagement.Application.Dtos.Responses;

public class EnrollmentResponseDto
{
    public Guid Id { get; set; }
    
    public DateTime EnrolledAt { get; set; }
    
    public string Status { get; set; } = "Active"; // Active / Dropped / Completed
    
    public string? Notes { get; set; }
    
    // Foreign Keys
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid GroupId { get; set; }
    public Guid BranchId { get; set; }
    public Guid PlanId { get; set; }
    
    // Navigation Properties
    public StudentResponseDto? Student { get; set; }
    public SubjectResponseDto? Subject { get; set; }
    public GroupResponseDto? Group { get; set; }
    public BranchResponseDto? Branch { get; set; }
    public PlanResponseDto? Plan { get; set; }
    
    public ICollection<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
}
