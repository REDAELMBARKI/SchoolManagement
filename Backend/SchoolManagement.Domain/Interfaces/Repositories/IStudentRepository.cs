using System;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Domain.Interfaces.Repositories.Common;

namespace SchoolManagement.Domain.Interfaces.Repositories;

public interface IStudentRepository : IRepository<Student> , IQuery<Student>
{

      

}
