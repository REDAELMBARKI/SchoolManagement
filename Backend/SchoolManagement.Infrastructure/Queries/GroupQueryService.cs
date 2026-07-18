using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class GroupQueryService : IGroupQueryService
{
    private readonly AppDbContext _context;

    public GroupQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Group>> GetAllAsync()
    {
        return await _context.Groups
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .Where(g => EF.Property<DateTime?>(g, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Group?> GetByIdAsync(Guid id)
    {
        return await _context.Groups
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .Where(g => EF.Property<DateTime?>(g, "DeletedAt") == null)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Groups
            .Where(g => EF.Property<DateTime?>(g, "DeletedAt") == null)
            .AnyAsync(g => g.Id == id);
    }

    public async Task<List<GroupResponseDto>> GetAllResponsesAsync()
    {
        var groups = await GetAllAsync();
        return groups.Select(GroupMapper.ToResponse).ToList();
    }

    public async Task<GroupResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var group = await GetByIdAsync(id);
        return group == null ? null : GroupMapper.ToResponse(group);
    }
}
