using SchoolManagement.Domain.Entities.Payment;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }
}