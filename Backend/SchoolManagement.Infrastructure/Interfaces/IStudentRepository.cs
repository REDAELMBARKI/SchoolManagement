using System;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Infrastructure.Interfaces ;


public interface IStudentRepository : IReadRepository<Student>, IWriteRepository<Student>
{

      

}
