using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface IAbsenceService
{
    Task<AbsenceResponseDto> CreateAsync(AbsenceCommand command);
    Task<AbsenceResponseDto> UpdateAsync(Guid id, UpdateAbsenceCommand command);
    Task DeleteAsync(Guid id);
    Task<AbsenceResponseDto> GetByIdAsync(Guid id);
    Task<List<AbsenceResponseDto>> GetAllAsync();
    Task<List<AbsenceResponseDto>> GetByStudentAsync(Guid studentId);
    Task<List<AbsenceResponseDto>> GetByScheduleAsync(Guid scheduleId);
}
