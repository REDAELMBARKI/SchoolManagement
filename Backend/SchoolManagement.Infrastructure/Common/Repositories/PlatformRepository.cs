using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class PlatformRepository : Repository<Platform>, IPlatformRepository
{
    public PlatformRepository(AppDbContext context) : base(context)
    {
    }

    public  async Task<List<Platform>> GetAllAsync()
    {
        return await Query().ToListAsync();
    }

      public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Set<Platform>()
            .AnyAsync(p => p.Slug == slug && p.DeletedAt == null);
    }


}

  