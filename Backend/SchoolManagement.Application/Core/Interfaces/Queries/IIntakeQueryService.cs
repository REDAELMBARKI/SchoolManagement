using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IIntakeQueryService : IEntityQuery<Intake>, ISluged
{
    Task<List<Intake>> GetAllIgnoreFiltersAsync();  // DEBUG: Bypass all filters
    Task<List<IntakeResponseDto>> GetAllResponsesAsync();
    Task<IntakeResponseDto?> GetResponseByIdAsync(Guid id);
    Task<List<IntakeResponseDto>> GetIntakesByStatusAsync(IntakeStatus status);
    Task<List<IntakeResponseDto>> GetIntakesByDateRangeAsync(DateTime startDate, DateTime endDate);
}
