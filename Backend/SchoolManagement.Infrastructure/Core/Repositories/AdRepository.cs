using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class AdRepository : Repository<Ad>, IAdRepository
{
    public AdRepository(AppDbContext context) : base(context)
    {
    }
}
