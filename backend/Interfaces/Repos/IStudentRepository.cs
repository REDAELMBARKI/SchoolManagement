using System;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Interfaces.Repos ;


public interface IStudentRepository : IReadRepository<Student>, IWriteRepository<Student>
{

      

}
