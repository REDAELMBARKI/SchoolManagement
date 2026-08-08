using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Academic.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<bool> ExistsBySlugAsync(string slug);
}
