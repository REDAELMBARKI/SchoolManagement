using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

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
            .Include(o => o.Gender)
            .Include(o => o.Branch)
            .Where(o => EF.Property<DateTime?>(o, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Opc?> GetByIdAsync(Guid id)
    {
        return await _context.Opcs
            .Include(o => o.Gender)
            .Include(o => o.Branch)
            .Where(o => EF.Property<DateTime?>(o, "DeletedAt") == null)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Opcs
            .Where(o => EF.Property<DateTime?>(o, "DeletedAt") == null)
            .AnyAsync(o => o.Id == id);
    }
}
