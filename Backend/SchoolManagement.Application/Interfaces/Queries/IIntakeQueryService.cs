using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IIntakeQueryService : IEntityQuery<Intake>, ISluged
{
    Task<List<IntakeResponseDto>> GetAllResponsesAsync();
    Task<IntakeResponseDto?> GetResponseByIdAsync(Guid id);
    Task<List<IntakeResponseDto>> GetIntakesByStatusAsync(IntakeStatus status);
    Task<List<IntakeResponseDto>> GetIntakesByBranchAsync(Guid branchId);
    Task<List<IntakeResponseDto>> GetIntakesByDateRangeAsync(DateTime startDate, DateTime endDate);
}
