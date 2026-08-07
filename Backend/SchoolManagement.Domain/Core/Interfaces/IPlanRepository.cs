using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IPlanRepository : IRepository<Plan>
{
    Task<List<Plan>> GetActiveAsync();
}
