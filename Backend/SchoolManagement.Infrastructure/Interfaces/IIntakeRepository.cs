using System;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Interfaces ;


public interface IIntakeRepository  : IReadRepository<Intake> , IWriteRepository<Intake>
{

}
