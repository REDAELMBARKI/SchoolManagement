namespace SchoolManagement.Application.Academic.Dtos.Commands;

public record UpdateGradeCommand
{
    public string EvaluationType { get; init; } = string.Empty;
    public float Score { get; init; }
    public float MaxScore { get; init; }
    public string? Comment { get; init; }
}
