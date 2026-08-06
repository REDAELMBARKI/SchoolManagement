using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class AdQueryService : IAdQueryService
{
    private readonly AppDbContext _context;

    public AdQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ad>> GetAllAsync()
    {
        return await _context.Ads
            .Include(a => a.Platform)
            .Include(a => a.Branch)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Ad?> GetByIdAsync(Guid id)
    {
        return await _context.Ads
            .Include(a => a.Platform)
            .Include(a => a.Branch)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Ads
            .AnyAsync(a => a.Id == id);
    }
}
