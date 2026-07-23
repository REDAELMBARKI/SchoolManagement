using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class StudentQueryService : IStudentQueryService
{
    private readonly AppDbContext _context;

    public StudentQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Users.OfType<Student>()
            .Include(s => s.Gender)
            .Include(s => s.Parents)
            .Include(s => s.Intake)
            .Include(s => s.Enrollments)
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Users.OfType<Student>()
            .Include(s => s.Gender)
            .Include(s => s.Parents)
            .Include(s => s.Intake)
            .Include(s => s.Enrollments)
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Users.OfType<Student>()
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .AnyAsync(s => s.Id == id);
    }

    public async Task<bool> IsExistsBySlugAsync(string slug)
    {
        return await _context.Users.OfType<Student>()
            .Where(s => EF.Property<DateTime?>(s, "DeletedAt") == null)
            .AnyAsync(s => s.Slug == slug);
    }

    public async Task<Student> FindByIdAsync(Guid id)
    {
        var student = await GetByIdAsync(id);
        if (student is null) throw new NotFoundException($"Student with id {id} not found");
        return student;
    }

    public async Task<List<StudentResponseDto>> GetAllResponsesAsync()
    {
        var students = await GetAllAsync();
        return students.Select(s => new StudentResponseDto
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Slug = s.Slug,
            Email = null,
            Phone = s.Phone,
            DateOfBirth = s.DateOfBirth,
            IntakeId = s.IntakeId
        }).ToList();
    }

    public async Task<StudentResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var student = await GetByIdAsync(id);
        if (student is null) return null;
        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Slug = student.Slug,
            Email = null,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth,
            IntakeId = student.IntakeId
        };
    }
}
