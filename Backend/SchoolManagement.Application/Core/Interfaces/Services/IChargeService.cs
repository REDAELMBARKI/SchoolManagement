using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IChargeService
{
    Task<List<ChargeResponseDto>> GetAllAsync();
    Task<ChargeResponseDto> GetByIdAsync(Guid id);
    Task<ChargeResponseDto> CreateAsync(ChargeCommand dto);
    Task<ChargeResponseDto> UpdateAsync(Guid id, UpdateChargeCommand command);
    Task DeleteAsync(Guid id);
}
