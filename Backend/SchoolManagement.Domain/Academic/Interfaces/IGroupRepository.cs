using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Academic.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<bool> ExistsBySlugAsync(string slug);
}
