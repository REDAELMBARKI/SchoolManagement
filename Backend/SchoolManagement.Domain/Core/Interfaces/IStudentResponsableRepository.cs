using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IStudentResponsableRepository : IRepository<StudentResponsable>
{
    Task<bool> IsExistsBySlugAsync(string slug);
}
