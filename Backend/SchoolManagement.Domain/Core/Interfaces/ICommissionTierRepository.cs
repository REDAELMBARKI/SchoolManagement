using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface ICommissionTierRepository
{
    Task<CommissionTier?> GetByIdAsync(Guid id);
    Task<List<CommissionTier>> GetAllAsync();
    Task<List<CommissionTier>> GetActiveAsync();
    Task<CommissionTier?> FindTierForSalesCountAsync(int salesCount);
    Task AddAsync(CommissionTier tier);
    Task UpdateAsync(CommissionTier tier);
    Task DeleteAsync(Guid id);
}
