using AutoMapper;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Mappers.Profiles ;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDto, Opc>() ;
        CreateMap<UserDto, CommercialAgent>() ;
        
         CreateMap<User, UserDto>()
            .IncludeAllDerived(); 

        CreateMap<UserDto, Employee>()
            .IncludeAllDerived();
    }
}