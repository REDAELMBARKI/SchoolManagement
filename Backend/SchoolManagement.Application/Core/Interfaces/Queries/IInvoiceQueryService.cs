using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IInvoiceQueryService : IEntityQuery<Invoice>
{
    Task<List<Invoice>> GetPastDueInvoicesAsync(DateTime? asOfDate = null);
    Task<List<Invoice>> GetInvoicesEndingWithinDaysAsync(int days = 3);
    Task<bool> HasRenewalInvoiceAsync(Guid enrollmentId, DateTime periodEnd);
    Task<Invoice?> GetLatestCancelableInvoiceByEnrollmentIdAsync(Guid enrollmentId);
}
