using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories.Common;

namespace SchoolManagement.Domain.Interfaces.Repositories;

public interface IIntakeRepository : IRepository<Intake>
{
    Task<Intake?> GetByIdForUpdateAsync(Guid id);
}
