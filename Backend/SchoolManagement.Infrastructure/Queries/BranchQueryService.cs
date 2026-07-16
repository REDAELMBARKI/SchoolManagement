using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class BranchQueryService : QueryServiceBase<Branch>, IBranchQueryService
{
    public BranchQueryService(AppDbContext context) : base(context)
    {
    }
}
