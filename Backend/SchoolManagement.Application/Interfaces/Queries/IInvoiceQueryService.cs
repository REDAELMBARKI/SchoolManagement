using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Application.Interfaces.Queries;

public interface IInvoiceQueryService : IEntityQuery<Invoice>
{
    Task<List<Invoice>> GetPastDueInvoicesAsync(DateTime? asOfDate = null);
}
