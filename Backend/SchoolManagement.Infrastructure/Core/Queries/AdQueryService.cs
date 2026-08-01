using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
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
            .Where(a => EF.Property<DateTime?>(a, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Ad?> GetByIdAsync(Guid id)
    {
        return await _context.Ads
            .Include(a => a.Platform)
            .Include(a => a.Branch)
            .Where(a => EF.Property<DateTime?>(a, "DeletedAt") == null)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Ads
            .Where(a => EF.Property<DateTime?>(a, "DeletedAt") == null)
            .AnyAsync(a => a.Id == id);
    }
}
