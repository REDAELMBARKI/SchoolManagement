using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class OpcQueryService : QueryServiceBase<Opc>, IOpcQueryService
{
    public OpcQueryService(AppDbContext context) : base(context)
    {
    }
}
