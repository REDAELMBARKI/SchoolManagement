using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

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
            .Where(b => EF.Property<DateTime?>(b, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Branch?> GetByIdAsync(Guid id)
    {
        return await _context.Branches
            .Where(b => EF.Property<DateTime?>(b, "DeletedAt") == null)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Branches
            .Where(b => EF.Property<DateTime?>(b, "DeletedAt") == null)
            .AnyAsync(b => b.Id == id);
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
}
