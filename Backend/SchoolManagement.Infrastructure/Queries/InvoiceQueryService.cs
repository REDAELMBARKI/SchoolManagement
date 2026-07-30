using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
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

    public async Task<List<Invoice>> GetPastDueInvoicesAsync(DateTime? asOfDate = null)
    {
        var targetDate = asOfDate ?? DateTime.UtcNow;

        return await _context.Invoices
            .Include(i => i.Charges)
            .Where(i => i.DueDate < targetDate
                     && i.Status != InvoiceStatus.Paid
                     && i.Status != InvoiceStatus.PastDue
                     && i.Status != InvoiceStatus.Waived
                     && i.Status != InvoiceStatus.Cancelled
                     && i.Charges.Any(c => c.Status == ChargeStatus.Active)
                     && i.PaidAmount < i.Charges
                         .Where(c => c.Status == ChargeStatus.Active)
                         .Sum(c => c.Amount))
            .ToListAsync();
    }

    public async Task<List<Invoice>> GetInvoicesEndingWithinDaysAsync(int days = 3)
    {
        var now = DateTime.UtcNow;
        var subscriptionEndDate = now.AddDays(days);

        return await _context.Invoices
            .Include(i => i.Charges)
            .Include(i => i.Enrollment)
                .ThenInclude(e => e.EnrollmentPlans)
                    .ThenInclude(ep => ep.Plan)
            .Where(i => i.PeriodEnd >= now
                     && i.PeriodEnd <= subscriptionEndDate
                     && i.Status != InvoiceStatus.Pending
                     && i.Status != InvoiceStatus.Cancelled
                     && i.Enrollment.Status == EnrollmentStatus.Active)
            .ToListAsync();
    }

    public async Task<bool> HasRenewalInvoiceAsync(Guid enrollmentId, DateTime periodEnd)
    {
        return await _context.Invoices
                    .AnyAsync(i => i.EnrollmentId == enrollmentId
                                && i.PeriodStart >= periodEnd
                                && i.Status != InvoiceStatus.Cancelled);
    }
}
