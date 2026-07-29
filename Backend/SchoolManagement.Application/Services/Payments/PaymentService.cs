using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public PaymentService(
        IPaymentRepository repository,
        IInvoiceRepository invoiceRepository,
        IPaymentQueryService paymentQueryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _invoiceRepository = invoiceRepository;
        _query = paymentQueryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<PaymentResponseDto>> GetAllAsync()
    {
        var payments = await _query.GetAllAsync();
        return payments.Select(p => PaymentMapper.ToResponse(p)).ToList();
    }

    public async Task<PaymentResponseDto?> GetByIdAsync(Guid id)
    {
        var payment = await _repository.GetByIdAsync(id);
        if (payment == null) return null;
        return PaymentMapper.ToResponse(payment);
    }

    public async Task<PaymentResponseDto> CreateAsync(RegistrationPaymentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;
        command.ReceivedByStaffId = _currentUserContext.NameIdentifier;

        var createdPayment = await CreatePaymentAsync(PaymentMapper.ToDomain(command));

        if (command.InvoiceId.HasValue && command.InvoiceId.Value != Guid.Empty)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(command.InvoiceId.Value);
            if (invoice != null)
            {
                invoice.AddPayment(createdPayment.Amount);
                await _invoiceRepository.UpdateAsync(invoice);
            }
        }

        return PaymentMapper.ToResponse(createdPayment);
    }

    public async Task<PaymentResponseDto> SettleChargeAsync(ChargeSettlementPaymentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;
        command.ReceivedByStaffId = _currentUserContext.NameIdentifier;

        var invoice = await _invoiceRepository.GetByIdAsync(command.InvoiceId);
        if (invoice == null)
            throw new NotFoundException($"No invoice found with id {command.InvoiceId}");

        if (invoice.BranchId != branchId)
            throw new DomainException("The invoice does not belong to the current branch.");

        var createdPayment = await CreatePaymentAsync(PaymentMapper.ToDomain(command));
        invoice.AddPayment(createdPayment.Amount);
        await _invoiceRepository.UpdateAsync(invoice);

        return PaymentMapper.ToResponse(createdPayment);
    }

    private async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        var createdPayment = await _repository.AddAsync(payment);
        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Payment),
            entityId: createdPayment.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(createdPayment));
        return createdPayment;
    }

    public async Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No payment found with id {id}");
        }

        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateEnrollmentId(command.EnrollmentId);
        existing.UpdateAmount(command.Amount);
        existing.UpdateTransferFees(command.TransferFees);
        existing.UpdateMethod(command.Method);
        existing.UpdatePaidAt(command.PaidAt);
        existing.UpdateStatus(command.Status);
        existing.UpdateBranchId(command.BranchId);
        existing.UpdateReceivedByStaffId(command.ReceivedByStaffId);
        existing.UpdateExternalReferenceCode(command.ExternalReferenceCode);
        existing.UpdateMethodDetailsJson(command.MethodDetailsJson ?? "{}");

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Payment),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return PaymentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existing != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Payment),
                entityId: existing.Id,
                branchId: _currentUserContext.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }
    }

    private static object CreateAuditSnapshot(Payment payment)
    {
        return new
        {
            payment.Id,
            payment.EnrollmentId,
            payment.InvoiceId,
            payment.Amount,
            payment.TransferFees,
            payment.Method,
            payment.Status,
            payment.PaidAt,
            payment.BranchId,
            payment.ReceivedByStaffId,
            payment.ExternalReferenceCode,
            payment.MethodDetailsJson,
            payment.CurrencyCode
        };
    }
}
