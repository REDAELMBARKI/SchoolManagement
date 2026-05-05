using SchoolManagement.Backend.Dtos.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Interfaces.Repos;

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
            .Include(s => s.Group)
            .Include(s => s.Subject)
            .Include(s => s.Level)
            .Include(s => s.StudentParents)
                .ThenInclude(sp => sp.Parent)
            .Include(s => s.Intake)
            .AsNoTracking().AsQueryable();
                
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
        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Gender = student.Gender.Name,
            Group = student.Group.Name,
            Level = student.Level.Name,
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
            Parents = student.StudentParents.Select(sp => sp.Parent).ToList()
        }; 
    }


    public async Task Destroy(int id )
    {
        var student = await Query().FirstOrDefaultAsync(s => s.Id == id);
        if(student == null ) return  ; 
        Context.Remove(student);
        await Context.SaveChangesAsync();

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

