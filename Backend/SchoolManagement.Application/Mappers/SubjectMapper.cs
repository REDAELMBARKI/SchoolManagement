using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class SubjectMapper
{
    public static Subject ToDomain(SubjectCommand command)
    {
        return Subject.Create(
            name: command.Name,
            slug: command.Slug,
            description: command.Description,
            branchId: command.BranchId
        );
    }

    public static SubjectResponseDto ToResponse(Subject subject)
    {
        return new SubjectResponseDto
        {
            Id = subject.Id,
            Slug = subject.Slug,
            Name = subject.Name,
        };
    }
}
