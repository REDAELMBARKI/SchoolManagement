using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>Loads payment with its Refunds collection included — needed for GetRefundableAmount().</summary>
    Task<Payment?> GetByIdWithRefundsAsync(Guid id);
}
