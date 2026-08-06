using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class LeadSourceQueryService : ILeadSourceQueryService
{
    private readonly AppDbContext _context;

    public LeadSourceQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeadSource>> GetAllAsync()
    {
        return await _context.LeadSources
            .Include(ls => ls.Branch)
            .OrderByDescending(ls => ls.CreatedAt)
            .ToListAsync();
    }

    public async Task<LeadSource?> GetByIdAsync(Guid id)
    {
        return await _context.LeadSources
            .Include(ls => ls.Branch)
            .FirstOrDefaultAsync(ls => ls.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.LeadSources
            .AnyAsync(ls => ls.Id == id);
    }
}
