using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IIntakeRepository : IRepository<Intake>
{
    Task<Intake?> GetIntakeByStudentId(Guid studentId);
}
