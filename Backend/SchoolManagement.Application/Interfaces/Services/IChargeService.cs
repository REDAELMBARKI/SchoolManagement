using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IChargeService
{
    Task<List<ChargeResponseDto>> GetAllAsync();
    Task<ChargeResponseDto?> GetByIdAsync(Guid id);
    Task<ChargeResponseDto> CreateAsync(ChargeCommand dto);
    Task<ChargeResponseDto> UpdateAsync(Guid id, UpdateChargeRequestDto dto);
    Task DeleteAsync(Guid id);
}