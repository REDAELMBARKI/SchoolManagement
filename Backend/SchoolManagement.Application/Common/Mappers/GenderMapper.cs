using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Mappers;

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
