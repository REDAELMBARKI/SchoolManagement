using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

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
