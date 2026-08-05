using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IStudentResponsableRepository : IRepository<StudentResponsable>
{
    Task<bool> IsExistsBySlugAsync(string slug);
}
