using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IIntakeRepository : IRepository<Intake>
{
    Task<Intake?> GetIntakeByStudentId(Guid studentId);
}
