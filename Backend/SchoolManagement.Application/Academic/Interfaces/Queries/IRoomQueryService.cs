using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface IRoomQueryService : IEntityQuery<Room>
{
    Task<Room?> GetByNameAsync(string name, Guid branchId);

}
