using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IStudentQueryService : IQuery<Student>
{
    Task<Student> FindByIdAsync(int id);
}
