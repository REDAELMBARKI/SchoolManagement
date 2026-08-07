using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class PayrollPaymentService : IPayrollPaymentService
{
    private readonly IPayrollPaymentRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IPayrollPaymentQueryService _query;

    public PayrollPaymentService(
        IPayrollPaymentRepository repository,
        IPayrollPaymentQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<PayrollPaymentResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _query.GetAllResponsesAsync();
    }

    public async Task<PayrollPaymentResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var payroll = await _query.GetResponseByIdAsync(id);
        if (payroll == null)
        {
            throw new NotFoundException($"Payroll payment with ID {id} not found.");
        }
        return payroll;
    }

    public async Task<List<PayrollPaymentResponseDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        // Use query service for non-tracking read operations
        var payrolls = await _query.GetByEmployeeIdAsync(employeeId);
        return payrolls.Select(PayrollPaymentMapper.ToResponse).ToList();
    }

    public async Task<List<PayrollPaymentResponseDto>> GetByPeriodAsync(int year, int month)
    {
        // Use query service for non-tracking read operations
        var payrolls = await _query.GetByPeriodAsync(year, month);
        return payrolls.Select(PayrollPaymentMapper.ToResponse).ToList();
    }

    public async Task<PayrollPaymentResponseDto> CreateAsync(PayrollPaymentCommand command)
    {
        // TODO: Get current user ID for ProcessedByStaffId
        var processedByStaffId = Guid.NewGuid(); // Placeholder - should come from authenticated user

        var payroll = PayrollPaymentMapper.ToDomain(command, _currentUserContext.BranchId, processedByStaffId);
        var created = await _repository.AddAsync(payroll);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "PayrollPayment",
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return PayrollPaymentMapper.ToResponse(created);
    }

    public async Task<PayrollPaymentResponseDto> MarkAsPaidAsync(Guid id, MarkPayrollPaidCommand command)
    {
        // Use repository for tracking operations
        var payroll = await _repository.GetByIdAsync(id);
        if (payroll == null)
        {
            throw new NotFoundException($"Payroll payment with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(payroll);

        payroll.MarkAsPaid(command.PaymentMethod, command.ReferenceCode);

        var updated = await _repository.UpdateAsync(payroll);

        await _auditLogService.StoreAsync(
            action: "MarkAsPaid",
            entityName: "PayrollPayment",
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return PayrollPaymentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations
        var payroll = await _repository.GetByIdAsync(id);
        if (payroll == null)
        {
            throw new NotFoundException($"Payroll payment with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(payroll);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "PayrollPayment",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    private static object CreateAuditSnapshot(Domain.Core.Entities.PayrollPayment payroll)
    {
        return new
        {
            payroll.Id,
            payroll.EmployeeId,
            payroll.GrossAmount,
            payroll.Bonus,
            payroll.Deductions,
            payroll.NetAmount,
            payroll.PayPeriodMonth,
            payroll.PayPeriodYear,
            payroll.Status,
            payroll.PaidAt,
            payroll.PaymentMethod,
            payroll.ReferenceCode,
            payroll.BranchId,
            payroll.ProcessedByStaffId,
            payroll.Notes
        };
    }
}
