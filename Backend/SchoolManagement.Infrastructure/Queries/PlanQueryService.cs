using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class PlanQueryService : IPlanQueryService
{
    private readonly AppDbContext _context;

    public PlanQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Plan>> GetAllAsync()
    {
        return await _context.Plans
            .Where(p => EF.Property<DateTime?>(p, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Plan?> GetByIdAsync(Guid id)
    {
        return await _context.Plans
            .Where(p => EF.Property<DateTime?>(p, "DeletedAt") == null)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Plans
            .Where(p => EF.Property<DateTime?>(p, "DeletedAt") == null)
            .AnyAsync(p => p.Id == id);
    }

    public async Task<List<PlanResponseDto>> GetAllResponsesAsync()
    {
        var plans = await GetAllAsync();
        return plans.Select(p => new PlanResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            DurationMonths = p.DurationMonths,
            BaseAmount = p.BaseAmount,
            DiscountPercent = p.DiscountPercent,
            IsActive = p.IsActive,
            RemainingAmountDueDate = p.RemainingAmountDueDate
        }).ToList();
    }

    public async Task<PlanResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var plan = await GetByIdAsync(id);
        return plan == null ? null : new PlanResponseDto
        {
            Id = plan.Id,
            Name = plan.Name,
            DurationMonths = plan.DurationMonths,
            BaseAmount = plan.BaseAmount,
            DiscountPercent = plan.DiscountPercent,
            IsActive = plan.IsActive,
            RemainingAmountDueDate = plan.RemainingAmountDueDate
        };
    }
}
