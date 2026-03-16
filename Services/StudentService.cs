using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolManagement.DTOs;
using SchoolManagement.Models;
using SchoolManagement.Repositories;

namespace SchoolManagement.Services;

public class StudentService
{
    private readonly StudentRepository _repository;
    private readonly StudentDTO _dto ;

    public StudentService(StudentRepository repository , StudentDTO dto )
    {
        _repository = repository;
        _dto  = dto ;
    }

    
    // public Student CreateStudent()
    // {

    // }


    // public async Task DeleteStudent()
    // {
    
    // }
}