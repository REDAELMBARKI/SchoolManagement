using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;
namespace SchoolManagement.Infrastructure.Core.Repositories;

public class CommissionRepository : Repository<Commission>, ICommissionRepository
{
    public CommissionRepository(AppDbContext context) : base(context) { }

    public async Task<List<Commission>> GetByEarnerAsync(Guid earnerId, EarnerType earnerType)
    {
        return await Query()
            .Where(c => c.EarnerId == earnerId && c.EarnerType == earnerType)
            .OrderByDescending(c => c.PeriodMonth)
            .ToListAsync();
    }

    public async Task<List<Commission>> GetByPeriodAsync(DateOnly periodMonth)
    {
        return await Query()
            .Where(c => c.PeriodMonth == periodMonth)
            .OrderBy(c => c.EarnerType)
            .ToListAsync();
    }

    public async Task<Commission?> GetAgentCommissionForPeriodAsync(Guid agentId, DateOnly periodMonth)
    {
        return await Query()
            .FirstOrDefaultAsync(c =>
                c.EarnerId == agentId &&
                c.EarnerType == EarnerType.CommercialAgent &&
                c.PeriodMonth == periodMonth);
    }

    public async Task<int> CountAgentEnrollmentsForMonthAsync(Guid agentId, int year, int month)
    {
        // Walk: Enrollment → Student → Intake → CommercialAgentId
        return await _context.Enrollments
            .Where(e =>
                EF.Property<DateTime?>(e, "DeletedAt") == null &&
                e.EnrolledAt.Year == year &&
                e.EnrolledAt.Month == month &&
                e.Student != null &&
                e.Student.Intake != null &&
                e.Student.Intake.CommercialAgentId == agentId)
            .CountAsync();
    }

    public async Task<bool> OpcCommissionExistsForEnrollmentAsync(Guid enrollmentId)
    {
        return await Query()
            .AnyAsync(c =>
                c.EarnerType == EarnerType.Opc &&
                c.SourceEnrollmentId == enrollmentId);
    }

    public async Task<List<Commission>> GetApprovedByPeriodAsync(DateOnly periodMonth)
    {
        return await Query()
            .Where(c => c.PeriodMonth == periodMonth && c.Status == CommissionStatus.Approved)
            .ToListAsync();
    }

    public async Task<Commission?> GetOpcCommissionByEnrollmentAsync(Guid enrollmentId)
    {
        return await Query()
            .FirstOrDefaultAsync(c =>
                c.EarnerType == EarnerType.Opc &&
                c.SourceEnrollmentId == enrollmentId);
    }
}
