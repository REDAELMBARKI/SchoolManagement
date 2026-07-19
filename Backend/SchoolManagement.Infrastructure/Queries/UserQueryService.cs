using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class UserQueryService : IUserQueryService
{
    private readonly AppDbContext _context;

    public UserQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Roles)
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Users
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .AnyAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    public async Task<bool> IsExistsByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => EF.Property<DateTime?>(u, "DeletedAt") == null)
            .AnyAsync(u => u.Email.Value == email);
    }


  
}
