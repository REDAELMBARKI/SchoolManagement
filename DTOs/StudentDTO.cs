using SchoolManagement.Http.Requests;
using SchoolManagement.Models;
namespace SchoolManagement.DTOs;

public class StudentDTO : Student
{
    public StudentDTO()
    {
        
    }

    public StudentDTO FromRequest(CreateStudentRequest data)
    {
        return new StudentDTO
        {
             FirstName = data.FirstName ,
             LastName = data.LastName ,
             Phone = data.Phone ,
             Email = data.Email ,
             Gender = data.Gender ,
             DateOfBirth = data.DateOfBirth ,
             PasswordHash =data.PasswordHash ,
        };
    }


    public StudentDTO FromBuissiness(StudentDTO old  , object fullFillmens)
    {
        return new StudentDTO
        {
        }
    }
}