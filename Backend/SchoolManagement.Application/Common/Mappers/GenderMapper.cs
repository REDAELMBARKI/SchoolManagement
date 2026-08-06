using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Mappers;

public static class GenderMapper
{
    public static Gender ToDomain(GenderCommand command)
    {
        return Gender.Create(
            name: command.Name,
            slug: command.Slug
        );
    }

    public static GenderResponseDto ToResponse(Gender gender)
    {
        return new GenderResponseDto
        {
            Id = gender.Id,
            Name = gender.Name,
            Slug = gender.Slug,
            CreatedAt = gender.CreatedAt
        };
    }
}
