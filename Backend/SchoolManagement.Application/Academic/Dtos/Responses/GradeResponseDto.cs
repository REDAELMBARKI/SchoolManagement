namespace SchoolManagement.Application.Academic.Dtos.Responses;

public class GradeResponseDto
{
    public Guid Id { get; set; }
    public string EvaluationType { get; set; } = string.Empty;
    public float Score { get; set; }
    public float MaxScore { get; set; }
    public DateTime EvaluationDate { get; set; }
    public string? Comment { get; set; }
    public Guid StudentId { get; set; }
    public Guid GroupTeacherId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
