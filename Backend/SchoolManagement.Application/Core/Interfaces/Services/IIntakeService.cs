namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IIntakeService
{
    Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync();
    Task<IntakeResponseDto> GetIntakeByIdAsync(Guid id);
    Task<IntakeResponseDto> AddIntakeAsync(IntakeCommand command);
    Task<IntakeResponseDto> UpdateAsync(Guid id, UpdateIntakeCommand command);
    Task DeleteIntakeAsync(Guid id);
}
