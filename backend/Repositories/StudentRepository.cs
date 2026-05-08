using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Interfaces.Repos;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

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
    
    public async Task<StudentResponseDto> AddAsync(Student student)
    {
        await this.AddAsync(student);
        await Context.SaveChangesAsync(); 
        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Gender = new GenderResponseDto
            {
                Id = student.Gender.Id,
                Slug = student.Gender.Slug,
                Name = student.Gender.Name
            },
            Parents = student.StudentParents.Select(sp => sp.Parent).ToList() ,
            DateOfBirth = student.DateOfBirth,
            IntakeId = student.IntakeId,
            Intake = student.Intake != null ? new IntakeResponseDto
            {
                Id = student.Intake.Id,
                FirstName = student.Intake.FirstName,
                LastName = student.Intake.LastName,
                Email = student.Intake.Email,
                Phone = student.Intake.Phone,
                IntakeDate = student.Intake.IntakeDate,
                Status = student.Intake.Status,
                Slug = student.Intake.Slug,
                CreatedAt = student.Intake.CreatedAt,
                DateOfBirth = student.Intake.DateOfBirth,
                FollowUpDate = student.Intake.FollowUpDate,
                Notes = student.Intake.Notes,
                TotalFees = student.Intake.TotalFees,
                AmountPaid = student.Intake.AmountPaid,
                IsIndependent = student.Intake.IsIndependent,
                Gender = new GenderResponseDto
                {
                    Id = student.Intake.Gender.Id,
                    Slug = student.Intake.Gender.Slug,
                    Name = student.Intake.Gender.Name
                },
                Subject = new SubjectResponseDto
                {
                    Id = student.Intake.Subject.Id,
                    Slug = student.Intake.Subject.Slug,
                    Name = student.Intake.Subject.Name,
                    Description = student.Intake.Subject.Description
                },
                Branch = new BranchResponseDto
                {
                    Id = student.Intake.Branch.Id,
                    Slug = student.Intake.Branch.Slug,
                    Name = student.Intake.Branch.Name,
                    City = student.Intake.Branch.City,
                    Address = student.Intake.Branch.Address,
                    Phone = student.Intake.Branch.Phone
                }
            } : null,
        }; 
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

