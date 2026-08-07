using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface ICommissionTierService
{
    Task<CommissionTierResponseDto> CreateAsync(CommissionTierCommand command);
    Task<CommissionTierResponseDto> UpdateAsync(Guid id, UpdateCommissionTierCommand command);
    Task DeleteAsync(Guid id);
    Task<CommissionTierResponseDto> GetByIdAsync(Guid id);
    Task<List<CommissionTierResponseDto>> GetAllAsync();
    Task<List<CommissionTierResponseDto>> GetActiveAsync();
    Task<CommissionTierResponseDto> ActivateAsync(Guid id);
    Task<CommissionTierResponseDto> DeactivateAsync(Guid id);
}
