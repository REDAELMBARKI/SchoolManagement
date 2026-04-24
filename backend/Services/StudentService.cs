using System.Threading.Tasks;
using SchoolManagement.Backend.Dtos.Responses;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Repositories;

namespace SchoolManagement.Backend.Services;

public class StudentService
{
    // private readonly StudentRepository SchoolManagement.Backend.epository;
    // private readonly StudentDTO SchoolManagement.Backend.to ;

    // public StudentService(StudentRepository repository , StudentDTO dto )
    // {
    //     SchoolManagement.Backend.epository = repository;
    //     SchoolManagement.Backend.to  = dto ;
    // }

    // public async Task<List<Student>>  getStudentList()
    // {
    //    return await SchoolManagement.Backend.epository.GetAllAsync();
    // }
    
    // public async Task<Student?>  getStudentById(int id)
    // {
    //   return await SchoolManagement.Backend.epository.FindByIdAsync(id);
    // }
    
    // public async Task<StudentResponseDto>  CreateStudent(StudentDTO studentDTO)
    // {   
    //    // check student level 
    //    // check group capasity 
    //    // chehck group module
    //    int groupId  = await  SchoolManagement.Backend.epository.checkGroupAvailabilityAsync(studentDTO.LevelId);
    //    Student student = new Student
    //    {
    //        FirstName = studentDTO.FirstName , 
    //        LastName = studentDTO.LastName , 
    //        DateOfBirth = studentDTO.DateOfBirth , 
    //        Email = studentDTO.Email , 
    //        Gender = studentDTO.Gender , 
    //        PasswordHash = studentDTO.PasswordHash ,
    //        Phone = studentDTO.Phone ,
    //        LevelId = studentDTO.LevelId ,
    //        GroupId = groupId
    //    };

    //    return await SchoolManagement.Backend.epository.AddAsync(student);
    // }


    // public async Task DeleteStudent()
    // {
    
    // }
}