using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class CommercialAgentQueryService : ICommercialAgentQueryService
{
    private readonly AppDbContext _context;

    public CommercialAgentQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommercialAgent>> GetAllAsync()
    {
        return await _context.CommercialAgents
            .Include(ca => ca.Gender)
            .Include(ca => ca.Branch)
                        .ToListAsync();
    }

    public async Task<CommercialAgent?> GetByIdAsync(Guid id)
    {
        return await _context.CommercialAgents
            .Include(ca => ca.Gender)
            .Include(ca => ca.Branch)
                        .FirstOrDefaultAsync(ca => ca.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.CommercialAgents
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
