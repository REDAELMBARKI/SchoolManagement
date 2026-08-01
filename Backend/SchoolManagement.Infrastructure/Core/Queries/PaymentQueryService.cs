using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;

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
