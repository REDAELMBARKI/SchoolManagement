using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class ChargeQueryService : IChargeQueryService
{
    private readonly AppDbContext _context;

    public ChargeQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Charge>> GetAllAsync()
    {
        return await _context.Charges
            .Include(c => c.Invoice)
            .ToListAsync();
    }

    public async Task<Charge?> GetByIdAsync(Guid id)
    {
        return await _context.Charges
            .Include(c => c.Invoice)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Charges
            .AnyAsync(c => c.Id == id);
    }
}
