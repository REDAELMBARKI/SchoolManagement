using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IUserRepository : IRepository<DomainUser>
{
    Task<bool> ExistsBySlugAsync(string slug);
    Task<bool> ExistsByEmailAsync(string email);
    Task<DomainUser?> GetByEmailAsync(string email);
    Task<DomainUser?> GetByApplicationUserIdAsync(string applicationUserId);
}
