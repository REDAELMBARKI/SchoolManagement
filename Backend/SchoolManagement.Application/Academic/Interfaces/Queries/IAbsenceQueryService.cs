using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface IAbsenceQueryService : IEntityQuery<Absence>
{
    Task<List<AbsenceResponseDto>> GetAllResponsesAsync();
    Task<AbsenceResponseDto?> GetResponseByIdAsync(Guid id);
    Task<List<Absence>> GetByStudentIdAsync(Guid studentId);
    Task<List<Absence>> GetByScheduleIdAsync(Guid scheduleId);
    Task<List<Absence>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}
