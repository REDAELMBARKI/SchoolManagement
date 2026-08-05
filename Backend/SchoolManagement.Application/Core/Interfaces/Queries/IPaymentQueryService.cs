using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries
{
    public interface IPaymentQueryService : IEntityQuery<Payment>
    {
    }
}
