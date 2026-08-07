using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class TeacherQueryService : ITeacherQueryService
{
    private readonly AppDbContext _context;

    public TeacherQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Teacher>> GetAllAsync()
    {
        return await _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .AsNoTracking()
            .Where(t => t.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Teacher?> GetByIdAsync(Guid id)
    {
        return await _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Teachers
            .AsNoTracking()
            .AnyAsync(t => t.Id == id && t.DeletedAt == null);
    }

    public async Task<List<TeacherResponseDto>> GetAllResponsesAsync()
    {
        var teachers = await GetAllAsync();
        return teachers.Select(TeacherMapper.ToResponse).ToList();
    }

    public async Task<TeacherResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var teacher = await GetByIdAsync(id);
        return teacher == null ? null : TeacherMapper.ToResponse(teacher);
    }

    public async Task<Teacher?> GetByEmailAsync(string email)
    {
        return await _context.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => (t.Email == null ? string.Empty : t.Email.Value) == email);
    }

    public async Task<Teacher?> GetBySlugAsync(string slug)
    {
        return await _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug && t.DeletedAt == null);
    }
}
