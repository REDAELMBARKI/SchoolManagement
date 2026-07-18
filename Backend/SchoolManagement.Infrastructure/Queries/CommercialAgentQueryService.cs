using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class CommercialAgentQueryService : ICommercialAgentQueryService
{
    private readonly AppDbContext _context;

    public CommercialAgentQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommercialAgent>> GetAllAsync()
    {
        return await _context.Users.OfType<CommercialAgent>()
            .Include(ca => ca.Gender)
            .Include(ca => ca.Branch)
            .Where(ca => EF.Property<DateTime?>(ca, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<CommercialAgent?> GetByIdAsync(Guid id)
    {
        return await _context.Users.OfType<CommercialAgent>()
            .Include(ca => ca.Gender)
            .Include(ca => ca.Branch)
            .Where(ca => EF.Property<DateTime?>(ca, "DeletedAt") == null)
            .FirstOrDefaultAsync(ca => ca.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Users.OfType<CommercialAgent>()
            .Where(ca => EF.Property<DateTime?>(ca, "DeletedAt") == null)
            .AnyAsync(ca => ca.Id == id);
    }

    public async Task<List<CommercialAgentResponseDto>> GetAllResponsesAsync()
    {
        var agents = await GetAllAsync();
        return agents.Select(CommercialAgentMapper.ToResponse).ToList();
    }

    public async Task<CommercialAgentResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var agent = await GetByIdAsync(id);
        return agent == null ? null : CommercialAgentMapper.ToResponse(agent);
    }
}
