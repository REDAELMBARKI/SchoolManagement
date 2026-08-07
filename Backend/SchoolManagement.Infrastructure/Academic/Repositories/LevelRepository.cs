using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Repositories;

public class LevelRepository : Repository<Level>, ILevelRepository
{
    public LevelRepository(AppDbContext context) : base(context)
    {
    }
}
