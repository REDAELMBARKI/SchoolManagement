using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class SubjectMapper
{
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
