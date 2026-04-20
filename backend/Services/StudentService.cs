using System.Threading.Tasks;
using _.Dtos.Responses;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolManagement.DTOs;
using SchoolManagement.Models;
using SchoolManagement.Repositories;

namespace SchoolManagement.Services;

public class StudentService
{
    // private readonly StudentRepository _repository;
    // private readonly StudentDTO _dto ;

    // public StudentService(StudentRepository repository , StudentDTO dto )
    // {
    //     _repository = repository;
    //     _dto  = dto ;
    // }

    // public async Task<List<Student>>  getStudentList()
    // {
    //    return await _repository.GetAllAsync();
    // }
    
    // public async Task<Student?>  getStudentById(int id)
    // {
    //   return await _repository.FindByIdAsync(id);
    // }
    
    // public async Task<StudentResponseDto>  CreateStudent(StudentDTO studentDTO)
    // {   
    //    // check student level 
    //    // check group capasity 
    //    // chehck group module
    //    int groupId  = await  _repository.checkGroupAvailabilityAsync(studentDTO.LevelId);
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

    //    return await _repository.AddAsync(student);
    // }


    // public async Task DeleteStudent()
    // {
    
    // }
}