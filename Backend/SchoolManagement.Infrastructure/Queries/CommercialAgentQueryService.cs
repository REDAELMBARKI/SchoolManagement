using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class CommercialAgentQueryService : QueryServiceBase<CommercialAgent>, ICommercialAgentQueryService
{
    public CommercialAgentQueryService(AppDbContext context) : base(context)
    {
    }
}
