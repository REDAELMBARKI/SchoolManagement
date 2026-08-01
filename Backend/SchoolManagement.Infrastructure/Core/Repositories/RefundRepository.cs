using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class RefundRepository : Repository<Refund>, IRefundRepository
{
    public RefundRepository(AppDbContext context) : base(context) { }

    public async Task<List<Refund>> GetByPaymentIdAsync(Guid paymentId)
    {
        return await Query()
            .Where(r => r.PaymentId == paymentId)
            .OrderByDescending(r => r.RefundedAt)
            .ToListAsync();
    }
}
