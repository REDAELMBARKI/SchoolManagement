using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Queries;

public class UserQueryService : IUserQueryService
{
    private readonly AppDbContext _context;

    public UserQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DomainUser>> GetAllAsync()
    {
        return await _context.DomainUsers
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<DomainUser?> GetByIdAsync(Guid id)
    {
        return await _context.DomainUsers
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.DomainUsers
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .AnyAsync(u => u.Id == id);
    }
}
