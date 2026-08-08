using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IBranchRepository : IRepository<Branch>
{
    Task<bool> ExistsBySlugAsync(string slug);
}