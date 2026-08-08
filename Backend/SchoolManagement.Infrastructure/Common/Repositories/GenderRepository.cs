using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class GenderRepository : Repository<Gender>, IGenderRepository
{
    public GenderRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Set<Gender>()
            .AnyAsync(g => g.Slug == slug && g.DeletedAt == null);
    }
}
