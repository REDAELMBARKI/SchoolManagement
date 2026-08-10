using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class UserRepository : Repository<DomainUser>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Set<DomainUser>().AnyAsync(u => u.Slug == slug);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Set<DomainUser>()
            .AnyAsync(u => u.Email != null && u.Email.Value == email);
    }

    public async Task<DomainUser?> GetByEmailAsync(string email)
    {
        return await _context.Set<DomainUser>()
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.Value == email);
    }

    public async Task<DomainUser?> GetByApplicationUserIdAsync(string applicationUserId)
    {
        return await _context.Set<DomainUser>()
            .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
    }
}
