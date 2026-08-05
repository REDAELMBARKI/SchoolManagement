using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Core.Queries
{
    internal class PaymentQueryService : IEntityQuery<Payment>
    {
        public Task<List<Payment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Payment?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
