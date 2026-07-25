using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IIntakeService
{
    Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync();
    Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id);
    Task<IntakeResponseDto> AddIntakeAsync(IntakeCommand command);
    Task<IntakeResponseDto?> UpdateAsync(Guid id, IntakeCommand command);
    Task DeleteIntakeAsync(Guid id);
}