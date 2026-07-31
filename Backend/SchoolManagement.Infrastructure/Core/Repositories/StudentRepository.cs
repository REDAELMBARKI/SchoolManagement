using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }

}
