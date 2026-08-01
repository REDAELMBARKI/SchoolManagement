using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Domain.Core.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>Loads payment with its Refunds collection included — needed for GetRefundableAmount().</summary>
    Task<Payment?> GetByIdWithRefundsAsync(Guid id);
}