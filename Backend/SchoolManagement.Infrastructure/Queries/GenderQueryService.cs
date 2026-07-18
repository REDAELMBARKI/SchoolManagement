using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class GenderQueryService : IGenderQueryService
{
    private readonly AppDbContext _context;

    public GenderQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Gender>> GetAllAsync()
    {
        return await _context.Genders
            .Where(g => EF.Property<DateTime?>(g, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<Gender?> GetByIdAsync(Guid id)
    {
        return await _context.Genders
            .Where(g => EF.Property<DateTime?>(g, "DeletedAt") == null)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Genders
            .Where(g => EF.Property<DateTime?>(g, "DeletedAt") == null)
            .AnyAsync(g => g.Id == id);
    }

    public async Task<List<GenderResponseDto>> GetAllResponsesAsync()
    {
        var genders = await GetAllAsync();
        return genders.Select(GenderMapper.ToResponse).ToList();
    }

    public async Task<GenderResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var gender = await GetByIdAsync(id);
        return gender == null ? null : GenderMapper.ToResponse(gender);
    }
}
