using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Application.Interfaces.Queries;

public interface IInvoiceQueryService : IEntityQuery<Invoice>
{
    Task<List<Invoice>> GetPastDueInvoicesAsync(DateTime? asOfDate = null);
    Task<List<Invoice>> GetInvoicesEndingWithinDaysAsync(int days = 3);
    Task<bool> HasRenewalInvoiceAsync(Guid enrollmentId, DateTime periodEnd);
    Task<Invoice?> GetLatestCancelableInvoiceByEnrollmentIdAsync(Guid enrollmentId);
}
