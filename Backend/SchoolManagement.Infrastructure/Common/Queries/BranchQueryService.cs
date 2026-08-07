using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Queries;

public class BranchQueryService : IBranchQueryService
{
    private readonly AppDbContext _context;

    public BranchQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Branch>> GetAllAsync()
    {
        return await _context.Branches
            .AsNoTracking()
            .Where(b => b.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Branch?> GetByIdAsync(Guid id)
    {
        return await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Branches
            .AsNoTracking()
            .AnyAsync(b => b.Id == id && b.DeletedAt == null);
    }

    public async Task<List<BranchResponseDto>> GetAllResponsesAsync()
    {
        var branches = await GetAllAsync();
        return branches.Select(BranchMapper.ToResponse).ToList();
    }

    public async Task<BranchResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var branch = await GetByIdAsync(id);
        return branch == null ? null : BranchMapper.ToResponse(branch);
    }

    public async Task<Branch?> GetByNameAsync(string name)
    {
        return await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Name == name && b.DeletedAt == null);
    }

    public async Task<Branch?> GetBySlugAsync(string slug)
    {
        return await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Slug == slug && b.DeletedAt == null);
    }

    public async Task<List<Branch>> GetByCityAsync(string city)
    {
        return await _context.Branches
            .AsNoTracking()
            .Where(b => b.City == city && b.DeletedAt == null)
            .ToListAsync();
    }
}
