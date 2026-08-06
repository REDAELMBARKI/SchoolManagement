using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class OpcQueryService : IOpcQueryService
{
    private readonly AppDbContext _context;

    public OpcQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Opc>> GetAllAsync()
    {
        return await _context.Opcs
            .Include(o => o.Branch)
            .Include(o => o.Gender)
            .OrderBy(o => o.FirstName)
            .ThenBy(o => o.LastName)
            .ToListAsync();
    }

    public async Task<Opc?> GetByIdAsync(Guid id)
    {
        return await _context.Opcs
            .Include(o => o.Branch)
            .Include(o => o.Gender)
            .Include(o => o.LeadSources)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Opcs
            .AnyAsync(o => o.Id == id);
    }
}
