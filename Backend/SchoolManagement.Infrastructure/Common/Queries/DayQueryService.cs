using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Queries;

public class DayQueryService : IDayQueryService
{
    private readonly AppDbContext _context;

    public DayQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Day?> GetByIdAsync(Guid id)
    {
        return await _context.Days
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Day>> GetAllAsync()
    {
        return await _context.Days
            .OrderBy(d => d.OrderIndex)
            .ToListAsync();
    }

    public async Task<Day?> GetByNameAsync(string name)
    {
        return await _context.Days
            .FirstOrDefaultAsync(d => d.Name == name);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Days.AnyAsync(r => r.Id == id);

    }
}
