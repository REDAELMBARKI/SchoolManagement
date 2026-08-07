using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class PayrollPaymentQueryService : IPayrollPaymentQueryService
{
    private readonly AppDbContext _context;

    public PayrollPaymentQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollPayment>> GetAllAsync()
    {
        return await _context.PayrollPayments
            .AsNoTracking()
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<PayrollPayment?> GetByIdAsync(Guid id)
    {
        return await _context.PayrollPayments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.PayrollPayments
            .AsNoTracking()
            .AnyAsync(p => p.Id == id && p.DeletedAt == null);
    }

    public async Task<List<PayrollPaymentResponseDto>> GetAllResponsesAsync()
    {
        var payrolls = await GetAllAsync();
        return payrolls
            .OrderByDescending(p => p.PayPeriodYear)
            .ThenByDescending(p => p.PayPeriodMonth)
            .Select(PayrollPaymentMapper.ToResponse)
            .ToList();
    }

    public async Task<PayrollPaymentResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var payroll = await GetByIdAsync(id);
        return payroll == null ? null : PayrollPaymentMapper.ToResponse(payroll);
    }

    public async Task<List<PayrollPayment>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.PayrollPayments
            .AsNoTracking()
            .Where(p => p.EmployeeId == employeeId && p.DeletedAt == null)
            .OrderByDescending(p => p.PayPeriodYear)
            .ThenByDescending(p => p.PayPeriodMonth)
            .ToListAsync();
    }

    public async Task<List<PayrollPayment>> GetByPeriodAsync(int year, int month)
    {
        return await _context.PayrollPayments
            .AsNoTracking()
            .Where(p => p.PayPeriodYear == year && p.PayPeriodMonth == month && p.DeletedAt == null)
            .ToListAsync();
    }
}
