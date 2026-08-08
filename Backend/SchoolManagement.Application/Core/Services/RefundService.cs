using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class RefundService : IRefundService
{
    private readonly IRefundRepository _repository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly ITransaction _transaction;

    public RefundService(
        IRefundRepository repository,
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService,
        ITransaction transaction)
    {
        _repository = repository;
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
        _transaction = transaction;
    }

    public async Task<RefundResponseDto> RefundPaymentAsync(Guid paymentId, RefundCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;
        command.RefundedByStaffId = _currentUserContext.NameIdentifier;

        await _transaction.BeginTransactionAsync();
        try
        {
            // Load payment with its refunds so GetRefundableAmount() is accurate
            var payment = await _paymentRepository.GetByIdWithRefundsAsync(paymentId)
                ?? throw new NotFoundException($"Payment {paymentId} not found.");

            if (payment.BranchId != branchId)
                throw new DomainException("Payment does not belong to the current branch.");

            if (payment.Status == Domain.Core.Enums.PaymentStatus.Refunded)
                throw new DomainException("This payment has already been fully refunded.");

            var refundable = payment.GetRefundableAmount();
            if (command.Amount > refundable)
                throw new DomainException(
                    $"Refund amount ({command.Amount:C}) exceeds the refundable balance ({refundable:C}).");

            // Create the refund record
            var refund = RefundMapper.ToDomain(command, paymentId);

            await _repository.AddAsync(refund);

            // If fully refunded, flip payment status
            if (payment.GetRefundableAmount() - command.Amount <= 0)
                payment.MarkAsRefunded();

            await _paymentRepository.UpdateAsync(payment);

            // Deduct from invoice if this payment was linked to one
            if (payment.InvoiceId.HasValue)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId.Value);
                if (invoice != null)
                {
                    invoice.DeductRefund(command.Amount);
                    await _invoiceRepository.UpdateAsync(invoice);
                }
            }

            await _auditLogService.StoreAsync(
                action: AuditLog.CreateAction(),
                entityName: nameof(Refund),
                entityId: refund.Id,
                branchId: branchId,
                newValues: CreateSnapshot(refund),
                message: $"Cash refund of {command.Amount:C} issued for payment {paymentId}. Reason: {command.Reason}");

            await _transaction.CommitTransactionAsync();

            return RefundMapper.ToResponse(refund);
        }
        catch
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<List<RefundResponseDto>> GetByPaymentIdAsync(Guid paymentId)
    {
        var refunds = await _repository.GetByPaymentIdAsync(paymentId);
        return refunds.Select(RefundMapper.ToResponse).ToList();
    }

    // ── Helpers ──────────────────────────────────────────────────

    private static object CreateSnapshot(Refund r) => new
    {
        r.Id,
        r.PaymentId,
        r.Amount,
        r.Reason,
        r.RefundedAt,
        r.RefundedByStaffId,
        r.BranchId
    };
}
