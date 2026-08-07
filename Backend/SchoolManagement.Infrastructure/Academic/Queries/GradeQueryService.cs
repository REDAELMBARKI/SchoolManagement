using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class GradeQueryService : IGradeQueryService
{
    private readonly AppDbContext _context;

    public GradeQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Grade>> GetAllAsync()
    {
        return await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Branch)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Grade?> GetByIdAsync(Guid id)
    {
        return await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Grades
            .AsNoTracking()
            .AnyAsync(g => g.Id == id && g.DeletedAt == null);
    }

    public async Task<List<GradeResponseDto>> GetAllResponsesAsync()
    {
        var grades = await GetAllAsync();
        return grades.Select(GradeMapper.ToResponse).ToList();
    }

    public async Task<GradeResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var grade = await GetByIdAsync(id);
        return grade == null ? null : GradeMapper.ToResponse(grade);
    }

    public async Task<List<Grade>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Branch)
            .AsNoTracking()
            .Where(g => g.StudentId == studentId && g.DeletedAt == null)
            .OrderByDescending(g => g.EvaluationDate)
            .ToListAsync();
    }

    public async Task<List<Grade>> GetByGroupTeacherIdAsync(Guid groupTeacherId)
    {
        return await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Branch)
            .AsNoTracking()
            .Where(g => g.GroupTeacherId == groupTeacherId && g.DeletedAt == null)
            .OrderByDescending(g => g.EvaluationDate)
            .ToListAsync();
    }
}
