using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Mappers;

public static class GradeMapper
{
    public static Grade ToDomain(GradeCommand command)
    {
        return Grade.Create(
            evaluationType: command.EvaluationType,
            score: command.Score,
            maxScore: command.MaxScore,
            evaluationDate: command.EvaluationDate,
            comment: command.Comment,
            studentId: command.StudentId,
            groupTeacherId: command.GroupTeacherId,
            branchId: command.BranchId);
    }

    public static GradeResponseDto ToResponse(Grade grade)
    {
        return new GradeResponseDto
        {
            Id = grade.Id,
            EvaluationType = grade.EvaluationType,
            Score = grade.Score,
            MaxScore = grade.MaxScore,
            EvaluationDate = grade.EvaluationDate,
            Comment = grade.Comment,
            StudentId = grade.StudentId,
            GroupTeacherId = grade.GroupTeacherId,
            BranchId = grade.BranchId,
            CreatedAt = grade.CreatedAt
        };
    }
}
