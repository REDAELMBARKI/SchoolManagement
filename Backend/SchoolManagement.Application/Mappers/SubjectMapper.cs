using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class SubjectMapper
{
    public static Subject ToDomain(SubjectRequestDto dto)
    {
        return Subject.Create(
            name: dto.Name,
            slug: dto.Slug,
            description: dto.Description,
            branchId: dto.BranchId
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
