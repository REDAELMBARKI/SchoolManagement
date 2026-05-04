using System;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Interfaces.Repos ;


public interface IStudentRepository : IReadRepository<Student>, IWriteRepository<Student>
{

      

}
