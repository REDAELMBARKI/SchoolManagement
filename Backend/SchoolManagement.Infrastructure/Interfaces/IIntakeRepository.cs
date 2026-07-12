using System;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Infrastructure.Interfaces ;


public interface IIntakeRepository  : IReadRepository<Intake> , IWriteRepository<Intake>
{

}
