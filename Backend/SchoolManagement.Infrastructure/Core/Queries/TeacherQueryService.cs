using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class TeacherQueryService : ITeacherQueryService
{
    private readonly AppDbContext _context;

    public TeacherQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Teacher?> GetByIdAsync(Guid id)
    {
        return await _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Teacher>> GetAllAsync()
    {
        var query = _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .AsQueryable();

        return await query.ToListAsync();
    }

    public async Task<Teacher?> GetBySlugAsync(string slug)
    {
        return await _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public async Task<Teacher?> GetByEmailAsync(string email)
    {
        return await _context.Teachers
            .Include(t => t.Branch)
            .Include(t => t.Gender)
            .FirstOrDefaultAsync(t => (t.Email.Value ?? string.Empty) == email);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Teachers.AnyAsync(t => t.Id == id);
    }
}
