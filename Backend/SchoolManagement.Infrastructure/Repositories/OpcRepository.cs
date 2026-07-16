using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class OpcRepository : Repository<Opc>, IOpcRepository
{
    public OpcRepository(AppDbContext context) : base(context)
    {
    }
}
