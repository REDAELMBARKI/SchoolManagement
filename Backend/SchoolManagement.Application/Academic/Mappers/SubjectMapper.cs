using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Mappers;

public static class SubjectMapper
{
    public static Subject ToDomain(SubjectCommand command, Guid branchId)
    {
        return Subject.Create(
            name: command.Name,
            slug: command.Slug,
            description: command.Description,
            branchId: branchId
        );
    }

    public static SubjectResponseDto ToResponse(Subject subject)
    {
        return new SubjectResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Slug = subject.Slug,
            Description = subject.Description,
            BranchId = subject.BranchId,
            CreatedAt = subject.CreatedAt
        };
    }
}
