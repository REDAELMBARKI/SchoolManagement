using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface ICommissionQueryService : IEntityQuery<Commission>
{
    Task<List<Commission>> GetByEarnerAsync(Guid earnerId, EarnerType earnerType);
    Task<List<Commission>> GetByPeriodAsync(DateOnly periodMonth);
    Task<Commission?> GetAgentCommissionForPeriodAsync(Guid agentId, DateOnly periodMonth);
    Task<List<Commission>> GetApprovedByPeriodAsync(DateOnly periodMonth);
    Task<Commission?> GetOpcCommissionByEnrollmentAsync(Guid enrollmentId);
}
