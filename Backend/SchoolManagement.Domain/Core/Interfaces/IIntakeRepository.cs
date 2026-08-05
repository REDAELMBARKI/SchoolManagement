using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IIntakeRepository : IRepository<Intake>
{
    Task<Intake?> GetIntakeByStudentId(Guid studentId);
}
