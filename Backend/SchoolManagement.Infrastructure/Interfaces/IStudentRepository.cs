using System;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Interfaces ;


public interface IStudentRepository : IReadRepository<Student>, IWriteRepository<Student>
{

      

}
