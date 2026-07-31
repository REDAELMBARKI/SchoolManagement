using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IIntakeQueryService : IEntityQuery<Intake>, ISluged
{
    Task<List<IntakeResponseDto>> GetAllResponsesAsync();
    Task<IntakeResponseDto?> GetResponseByIdAsync(Guid id);
    Task<List<IntakeResponseDto>> GetIntakesByStatusAsync(IntakeStatus status);
    Task<List<IntakeResponseDto>> GetIntakesByBranchAsync(Guid branchId);
    Task<List<IntakeResponseDto>> GetIntakesByDateRangeAsync(DateTime startDate, DateTime endDate);
}
