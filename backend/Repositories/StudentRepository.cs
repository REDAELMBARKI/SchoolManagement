using _.Dtos.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.DTOs;
using SchoolManagement.Interfaces;
using SchoolManagement.Models;

namespace SchoolManagement.Repositories;

public class StudentRepository 
{
    AppDbContext _context ;
    public StudentRepository(AppDbContext context)
    {
        _context  = context ;
    }

    public async Task<List<Student>> GetAllAsync()
    { 
        return  await _context.Students.ToListAsync() ;
    }

    public async Task<Student?>  FindByIdAsync(int id)
    {
        return await _context.Students.FindAsync(id);
    }
    
    public async Task<StudentResponseDto> AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync(); 
        return new StudentResponseDto(
            student.Id,
            student.FirstName ,
            student.LastName , 
            student.Gender.Name,
            student.Group.Name ,
            student.Level.Name ,
            student.DateOfBirth ,
            student.Parents
        ); 
    }


    public async Task Destroy(int id )
    {
        var student = await _context.Students.FindAsync(id);
        if(student == null ) return  ; 
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

    }

    public async Task Update(int id )
    {
        var student = await _context.Students.FindAsync(id);
        if(student == null ) return  ; 
        _context.Students.Update(student) ;
        await _context.SaveChangesAsync();
        
    }


    public async Task<int> checkGroupAvailabilityAsync(int levelId)
    {
       var group = await _context.Groups.Where(g => g.Students.Count() < g.Capacity).FirstAsync();
       return group.Id ;
    }
}