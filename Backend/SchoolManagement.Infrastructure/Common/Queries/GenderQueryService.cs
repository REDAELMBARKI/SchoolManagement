using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Queries;

public class GenderQueryService : IGenderQueryService
{
    private readonly AppDbContext _context;

    public GenderQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Gender>> GetAllAsync()
    {
        return await _context.Genders
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<Gender?> GetByIdAsync(Guid id)
    {
        return await _context.Genders
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Genders
            .AnyAsync(g => g.Id == id);
    }
}
