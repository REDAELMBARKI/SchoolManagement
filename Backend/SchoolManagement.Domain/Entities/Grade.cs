using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Grade : AggregateRoot
{
    public string EvaluationType { get; private set; } = string.Empty;
    public float Score { get; private set; } = 0f;
    public float MaxScore { get; private set; } = 100f;
    public DateTime EvaluationDate { get; private set; } = DateTime.UtcNow;
    public string? Comment { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid GroupTeacherId { get; private set; }
    public Guid BranchId { get; private set; }
    public virtual Branch Branch { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;

    private Grade() { }

    public static Grade Create(string evaluationType, float score, float maxScore, DateTime evaluationDate, string? comment, Guid studentId, Guid groupTeacherId, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(evaluationType))
        {
            throw new DomainException("Evaluation type cannot be empty.");
        }
        if (score < 0)
        {
            throw new DomainException("Score cannot be negative.");
        }
        if (maxScore <= 0)
        {
            throw new DomainException("Max score must be greater than zero.");
        }
        if (studentId == Guid.Empty)
        {
            throw new DomainException("Student ID must not be empty.");
        }
        if (groupTeacherId == Guid.Empty)
        {
            throw new DomainException("Group Teacher ID must not be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        return new Grade
        {
            EvaluationType = evaluationType,
            Score = score,
            MaxScore = maxScore,
            EvaluationDate = evaluationDate,
            Comment = comment,
            StudentId = studentId,
            GroupTeacherId = groupTeacherId,
            BranchId = branchId
        };
    }

    public void UpdateEvaluationType(string evaluationType)
    {
        if (string.IsNullOrWhiteSpace(evaluationType))
        {
            throw new DomainException("Evaluation type cannot be empty.");
        }
        EvaluationType = evaluationType;
    }

    public void UpdateScore(float score)
    {
        if (score < 0)
        {
            throw new DomainException("Score cannot be negative.");
        }
        Score = score;
    }

    public void UpdateMaxScore(float maxScore)
    {
        if (maxScore <= 0)
        {
            throw new DomainException("Max score must be greater than zero.");
        }
        MaxScore = maxScore;
    }

    public void UpdateEvaluationDate(DateTime evaluationDate)
    {
        EvaluationDate = evaluationDate;
    }

    public void UpdateComment(string? comment)
    {
        Comment = comment;
    }

    public void UpdateStudentId(Guid studentId)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("Student ID must not be empty.");
        }
        StudentId = studentId;
    }

    public void UpdateGroupTeacherId(Guid groupTeacherId)
    {
        if (groupTeacherId == Guid.Empty)
        {
            throw new DomainException("Group Teacher ID must not be empty.");
        }
        GroupTeacherId = groupTeacherId;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }
}
