using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IDayQueryService : IEntityQuery<Day>
{
    Task<Day?> GetByNameAsync(string name);

}
