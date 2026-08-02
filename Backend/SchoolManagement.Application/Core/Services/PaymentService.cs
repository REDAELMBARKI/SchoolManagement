using Microsoft.Extensions.Options;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Options;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly BillingOptions _billingOptions;

    public PaymentService(
        IPaymentRepository repository,
        IInvoiceRepository invoiceRepository,
        IEnrollmentRepository enrollmentRepository,
        IPaymentQueryService paymentQueryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService,
        IOptions<BillingOptions> billingOptions)
    {
        _repository = repository;
        _invoiceRepository = invoiceRepository;
        _enrollmentRepository = enrollmentRepository;
        _query = paymentQueryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
        _billingOptions = billingOptions.Value;
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
                var paymentResult = invoice.AddPayment(createdPayment.Amount);
                await _invoiceRepository.UpdateAsync(invoice);

                await StoreOverpaymentAsCreditAsync(invoice.EnrollmentId, paymentResult.OverpaymentAmount);
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
        var paymentResult = invoice.AddPayment(createdPayment.Amount);
        await _invoiceRepository.UpdateAsync(invoice);

        await StoreOverpaymentAsCreditAsync(invoice.EnrollmentId, paymentResult.OverpaymentAmount);

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

    private async Task StoreOverpaymentAsCreditAsync(Guid enrollmentId, decimal overpaymentAmount)
    {
        if (overpaymentAmount <= 0 || !_billingOptions.AllowOverpaymentToCredit)
            return;

        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
        if (enrollment == null)
            throw new NotFoundException($"No enrollment found with id {enrollmentId}");

        enrollment.AddCredit(overpaymentAmount);
        await _enrollmentRepository.UpdateAsync(enrollment);
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
