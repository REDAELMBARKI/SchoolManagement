namespace SchoolManagement.Application.Academic.Dtos.Requests;

public record UpdateGradeRequestDto
{
    public string EvaluationType { get; init; } = string.Empty;
    public float Score { get; init; }
    public float MaxScore { get; init; }
    public string? Comment { get; init; }
}
