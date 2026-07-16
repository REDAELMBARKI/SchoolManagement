using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class AdQueryService : QueryServiceBase<Ad>, IAdQueryService
{
    public AdQueryService(AppDbContext context) : base(context)
    {
    }

    public Task<List<Ad>> GetAllAsync()
    {
        return Query().ToListAsync();
    }

    public Task<Ad?> GetByIdAsync(int id)
    {
        return Query().FirstOrDefaultAsync(a => a.Id == id);
    }
}
