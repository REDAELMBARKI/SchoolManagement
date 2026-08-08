using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IPlatformRepository : IRepository<Platform>
{
    Task<List<Platform>> GetAllAsync();
    Task<bool> ExistsBySlugAsync(string slug);
}
