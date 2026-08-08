using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Academic.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<bool> ExistsBySlugAsync(string slug);
}
