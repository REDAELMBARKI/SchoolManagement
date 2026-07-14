using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Application.Dtos;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Infrastructure.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentRepository :  Repository<Student> 
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }


    protected override IQueryable<Student> Query()
    {
        return this.Context.Users.OfType<Student>()
            .Include(s => s.Gender)
            .Include(s => s.StudentParents)
                .ThenInclude(sp => sp.Parent)
            .Include(s => s.Intake)
            .Include(s => s.Enrollments)
            .AsNoTracking().AsQueryable();
                
    }
    public async Task<List<Student>> GetAllAsync()
    { 
        return  await this.Query().ToListAsync() ;
    }

    public async Task<Student> FindByIdAsync(int id)
    {
        var student = await this.Query().FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) throw new NotFoundException($"Student with id {id} not found");
        return student;
    }
    
    public async Task<Student> AddAsync(Student student)
    {
        await this.AddAsync(student);
        await Context.SaveChangesAsync();
        await Context.Entry(student).Reference(s => s.Gender).LoadAsync();
        await Context.Entry(student).Collection(s => s.Enrollments).LoadAsync();
        if (student.IntakeId != null) 
            await Context.Entry(student).Reference(s => s.Intake).LoadAsync();  

        return student;  
    }


    public async Task DeleteAsync(int id)
    {
        var student = await Query().FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) throw new NotFoundException($"Student with id {id} not found");
        student.DeletedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Student student)
    {
        var dbStudent = await Context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
        if (dbStudent is null) throw new NotFoundException($"Student with id {id} not found");
        student.Id = dbStudent.Id;
        Context.Entry(dbStudent).CurrentValues.SetValues(student);
        await Context.SaveChangesAsync();
    }


    // public async Task<int> checkGroupAvailabilityAsync(int levelId)
    // {
    //    var group = await Context.Groups.Where(g => g.Users.OfType<Student>().Count() < g.Capacity).FirstAsync();
    //    return group.Id ;
    // }
}

