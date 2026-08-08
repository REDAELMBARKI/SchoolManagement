using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IOpcRepository : IRepository<Opc>
{
    Task<bool> ExistsBySlugAsync(string slug);
}
