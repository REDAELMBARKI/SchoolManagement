using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class OpcRepository : Repository<Opc>, IOpcRepository
{
    public OpcRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Set<Opc>().AnyAsync(o => o.Slug == slug);
    }
}
