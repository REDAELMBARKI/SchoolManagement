using SchoolManagement.Domain.Entities.Payment;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Infrastructure.Queries
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
