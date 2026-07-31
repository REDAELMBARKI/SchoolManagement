using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IIntakeService
{
    Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync();
    Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id);
    Task<IntakeResponseDto> AddIntakeAsync(IntakeCommand command);
    Task<IntakeResponseDto?> UpdateAsync(Guid id, UpdateIntakeCommand command);
    Task DeleteIntakeAsync(Guid id);
}
