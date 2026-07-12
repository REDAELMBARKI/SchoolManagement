namespace SchoolManagement.Backend.Dtos.Responses;

public class EnrollmentResponseDto
{
    public int Id { get; set; }
    
    public DateTime EnrolledAt { get; set; }
    
    public string Status { get; set; } = "Active"; // Active / Dropped / Completed
    
    // Foreign Keys
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int GroupId { get; set; }
    public int BranchId { get; set; }
    public int PlanId { get; set; }
    
    // Navigation Properties
    public StudentResponseDto? Student { get; set; }
    public SubjectResponseDto? Subject { get; set; }
    public GroupResponseDto? Group { get; set; }
    public BranchResponseDto? Branch { get; set; }
    public PlanResponseDto? Plan { get; set; }
    
    public ICollection<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
}
