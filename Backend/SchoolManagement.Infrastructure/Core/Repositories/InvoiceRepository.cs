using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await Query()
            .Include(i => i.Charge)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}
