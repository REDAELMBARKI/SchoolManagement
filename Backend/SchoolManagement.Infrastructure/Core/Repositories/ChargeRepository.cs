using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class ChargeRepository : Repository<Charge>, IChargeRepository
{
    public ChargeRepository(AppDbContext context) : base(context)
    {
    }
}