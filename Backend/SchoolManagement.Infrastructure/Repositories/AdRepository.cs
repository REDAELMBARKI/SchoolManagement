using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class AdRepository : Repository<Ad>, IAdRepository
{
    public AdRepository(AppDbContext context) : base(context)
    {
    }
}
