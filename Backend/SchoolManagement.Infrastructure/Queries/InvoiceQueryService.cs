using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

public class InvoiceQueryService : IInvoiceQueryService
{
    private readonly AppDbContext _context;

    public InvoiceQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Charges)
            .Include(i => i.Payments)
            .Include(i => i.Enrollment)
            .Include(i => i.Branch)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Charges)
            .Include(i => i.Payments)
            .Include(i => i.Enrollment)
            .Include(i => i.Branch)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Invoices
            .AnyAsync(i => i.Id == id);
    }
}
