using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class CommissionQueryService : ICommissionQueryService
{
    private readonly AppDbContext _context;

    public CommissionQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Commission>> GetAllAsync()
    {
        return await _context.Commissions
            .AsNoTracking()
            .Where(c => c.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Commission?> GetByIdAsync(Guid id)
    {
        return await _context.Commissions
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Commissions
            .AsNoTracking()
            .AnyAsync(c => c.Id == id && c.DeletedAt == null);
    }

    public async Task<List<Commission>> GetByEarnerAsync(Guid earnerId, EarnerType earnerType)
    {
        return await _context.Commissions
            .AsNoTracking()
            .Where(c => c.EarnerId == earnerId && c.EarnerType == earnerType && c.DeletedAt == null)
            .OrderByDescending(c => c.PeriodMonth)
            .ToListAsync();
    }

    public async Task<List<Commission>> GetByPeriodAsync(DateOnly periodMonth)
    {
        return await _context.Commissions
            .AsNoTracking()
            .Where(c => c.PeriodMonth == periodMonth && c.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Commission?> GetAgentCommissionForPeriodAsync(Guid agentId, DateOnly periodMonth)
    {
        return await _context.Commissions
            .AsNoTracking()
            .FirstOrDefaultAsync(c => 
                c.EarnerId == agentId && 
                c.EarnerType == EarnerType.CommercialAgent && 
                c.PeriodMonth == periodMonth && 
                c.DeletedAt == null);
    }

    public async Task<List<Commission>> GetApprovedByPeriodAsync(DateOnly periodMonth)
    {
        return await _context.Commissions
            .Where(c => c.PeriodMonth == periodMonth && c.Status == CommissionStatus.Approved && c.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Commission?> GetOpcCommissionByEnrollmentAsync(Guid enrollmentId)
    {
        return await _context.Commissions
            .AsNoTracking()
            .FirstOrDefaultAsync(c => 
                c.SourceEnrollmentId == enrollmentId && 
                c.EarnerType == EarnerType.Opc && 
                c.DeletedAt == null);
    }
}
