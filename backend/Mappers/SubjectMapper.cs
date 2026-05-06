using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public static class SubjectMapper
{
    public static SubjectResponseDto MapSubject(Subject subject)
    {
        return new SubjectResponseDto
        {
            Id = subject.Id,
            Slug = subject.Slug,
            Name = subject.Name,
            Description = subject.Description
        };
    }
}
