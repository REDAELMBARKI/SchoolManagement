using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class CommissionTierRepository : ICommissionTierRepository
{
    private readonly AppDbContext _context;

    public CommissionTierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CommissionTier?> GetByIdAsync(Guid id)
    {
        return await _context.CommissionTiers
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<CommissionTier>> GetAllAsync()
    {
        return await _context.CommissionTiers
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.MinSalesCount)
            .ToListAsync();
    }

    public async Task<List<CommissionTier>> GetActiveAsync()
    {
        return await _context.CommissionTiers
            .Where(t => t.IsActive)
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.MinSalesCount)
            .ToListAsync();
    }

    public async Task<CommissionTier?> FindTierForSalesCountAsync(int salesCount)
    {
        return await _context.CommissionTiers
            .Where(t => t.IsActive)
            .Where(t => salesCount >= t.MinSalesCount &&
                       (t.MaxSalesCount == null || salesCount <= t.MaxSalesCount))
            .OrderBy(t => t.MinSalesCount)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CommissionTier tier)
    {
        await _context.CommissionTiers.AddAsync(tier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CommissionTier tier)
    {
        _context.CommissionTiers.Update(tier);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tier = await GetByIdAsync(id);
        if (tier != null)
        {
            _context.CommissionTiers.Remove(tier);
            await _context.SaveChangesAsync();
        }
    }
}
