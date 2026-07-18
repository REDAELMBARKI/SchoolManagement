using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class GenderMapper
{
    public static Gender ToDomain(GenderRequestDto dto)
    {
        return Gender.Create(name: dto.Name, slug: dto.Slug);
    }

    public static GenderResponseDto ToResponse(Gender gender)
    {
        return new GenderResponseDto
        {  
            Id = gender.Id,
            Slug = gender.Slug,
            Name = gender.Name 
        };
    }
}
