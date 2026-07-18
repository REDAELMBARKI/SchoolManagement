using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IIntakeService
{
    Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync();
    Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id);
    Task<IntakeResponseDto> AddIntakeAsync(IntakeRequestDto intakeDto);
    Task<IntakeResponseDto?> UpdateAsync(Guid id, IntakeRequestDto intakeDto);
    Task DeleteIntakeAsync(Guid id);
}