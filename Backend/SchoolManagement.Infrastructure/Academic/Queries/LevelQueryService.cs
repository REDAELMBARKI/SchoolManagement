using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class LevelQueryService : ILevelQueryService
{
    private readonly AppDbContext _context;

    public LevelQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Level>> GetAllAsync()
    {
        return await _context.Levels
            .Include(l => l.Branch)
            .AsNoTracking()
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<Level?> GetByIdAsync(Guid id)
    {
        return await _context.Levels
            .Include(l => l.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Levels
            .AsNoTracking()
            .AnyAsync(l => l.Id == id);
    }

    public async Task<List<LevelResponseDto>> GetAllResponsesAsync()
    {
        var levels = await GetAllAsync();
        return levels.Select(LevelMapper.ToResponse).ToList();
    }

    public async Task<LevelResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var level = await GetByIdAsync(id);
        return level == null ? null : LevelMapper.ToResponse(level);
    }

    public async Task<Level?> GetByNameAsync(string name)
    {
        return await _context.Levels
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Name == name);
    }
}
