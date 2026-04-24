using SchoolManagement.Backend.Dtos.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Backend.Dtos.Responses;
using SchoolManagement.Backend.Interfaces.Repos;

namespace SchoolManagement.Backend.Repositories;

public class StudentRepository :  Repository<Student> 
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }


    protected override IQueryable<Student> Query()
    {
        return this.Context.Users.OfType<Student>().AsNoTracking().AsQueryable();
                
    }
    public async Task<List<Student>> GetAllAsync()
    { 
        return  await this.Query().ToListAsync() ;
    }

    public async Task<Student?>  FindByIdAsync(int id)
    {
        return await this.Query().FirstAsync(s => s.Id == id) ;
    }
    
    public async Task<StudentResponseDto> AddAsync(Student student)
    {
        await this.AddAsync(student);
        await Context.SaveChangesAsync(); 
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
        // var student = await Context.Users.OfType<Student>().FindAsync(id);
        // if(student == null ) return  ; 
        // Context.Users.OfType<Student>().Remove(student);
        // await Context.SaveChangesAsync();

    }

    public async Task Update(int id )
    {
        // var student = await Context.Users.OfType<Student>().FindAsync(id);
        // if(student == null ) return  ; 
        // Context.Users.OfType<Student>().Update(student) ;
        // await Context.SaveChangesAsync();
        
    }


    // public async Task<int> checkGroupAvailabilityAsync(int levelId)
    // {
    //    var group = await Context.Groups.Where(g => g.Users.OfType<Student>().Count() < g.Capacity).FirstAsync();
    //    return group.Id ;
    // }
}

