using System;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Interfaces.Repos ;


public interface IIntakeRepository  : IReadRepository<Intake> , IWriteRepository<Intake>
{

}
