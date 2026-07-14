using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public class GenderMapper
{
    public static GenderResponseDto MapGender(Gender gender)
    {
        return new GenderResponseDto
        {  
            Id = gender.Id ,
            Slug = gender.Slug ,
            Name = gender.Name 
        }   ;
    }
}
