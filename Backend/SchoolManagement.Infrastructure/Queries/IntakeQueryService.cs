using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class IntakeQueryService : IIntakeQueryService
{
    private readonly AppDbContext _context;

    public IntakeQueryService(AppDbContext context)
    {
        _context = context;
    }

    // Entity-specific methods (return entity)
    public async Task<List<Intake>> GetAllAsync()
    {
        return await _context.Intakes
            .Include(i => i.Gender)
            .Include(i => i.LeadSource)
            .Include(i => i.Subject)
            .Include(i => i.CommercialAgent)
            .Include(i => i.Branch)
            .ToListAsync();
    }

    public async Task<Intake?> GetByIdAsync(Guid id)
    {
        return await _context.Intakes
            .Include(i => i.Gender)
            .Include(i => i.LeadSource)
            .Include(i => i.Subject)
            .Include(i => i.CommercialAgent)
            .Include(i => i.Branch)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Intakes
            .AnyAsync(i => i.Id == id);
    }

    // Response-specific methods (use entity method then map)
    public async Task<List<IntakeResponseDto>> GetAllResponsesAsync()
    {
        var intakes = await GetAllAsync();
        return intakes.Select(IntakeMapper.ToResponse).ToList();
    }

    public async Task<IntakeResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var intake = await GetByIdAsync(id);
        return intake == null ? null : IntakeMapper.ToResponse(intake);
    }

    // ISluged implementation
    public async Task<bool> IsExistsBySlug(string slug)
    {
        return await _context.Intakes
            .AnyAsync(i => i.Slug == slug);
    }

    // Keep IsExistBySlug for backward compatibility
    public async Task<bool> IsExistBySlug(string slug)
    {
        return await IsExistsBySlug(slug);
    }
}
