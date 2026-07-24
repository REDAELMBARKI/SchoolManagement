using SchoolManagement.Domain.Entities.Payment;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces.Queries
{
    public interface IPaymentQueryService : IEntityQuery<Payment>
    {
    }
}
