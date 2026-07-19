using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IUserQueryService : IEntityQuery<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsExistsByEmailAsync(string email);
  
}
