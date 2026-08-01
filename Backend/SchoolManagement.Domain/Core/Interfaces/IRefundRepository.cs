using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IRefundRepository : IRepository<Refund>
{
    /// <summary>Returns all refunds for a given payment.</summary>
    Task<List<Refund>> GetByPaymentIdAsync(Guid paymentId);
}
