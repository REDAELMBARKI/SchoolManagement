namespace SchoolManagement.Application.Academic.Dtos.Requests;

public record CreateGradeRequestDto
{
    public string EvaluationType { get; init; } = string.Empty;
    public float Score { get; init; }
    public float MaxScore { get; init; }
    public DateTime EvaluationDate { get; init; }
    public string? Comment { get; init; }
    public Guid StudentId { get; init; }
    public Guid GroupTeacherId { get; init; }
}
