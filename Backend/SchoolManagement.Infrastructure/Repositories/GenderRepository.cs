using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class GenderRepository : Repository<Gender>, IGenderRepository
{
    public GenderRepository(AppDbContext context) : base(context)
    {
    }
}
