using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class CommercialAgentRepository : Repository<CommercialAgent>, ICommercialAgentRepository
{
    public CommercialAgentRepository(AppDbContext context) : base(context)
    {
    }
}
