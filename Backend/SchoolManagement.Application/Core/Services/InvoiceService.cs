using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Options;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IInvoiceQueryService _query;
    private readonly IEnrollmentQueryService _enrollmentQuery;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<InvoiceService> _logger;
    private readonly BillingOptions _billingOptions;

    public InvoiceService(
        IInvoiceRepository repository,
        IInvoiceQueryService query,
        IEnrollmentQueryService enrollmentQuery,
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        IPaymentRepository paymentRepository,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext,
        ILogger<InvoiceService> logger,
        IOptions<BillingOptions> billingOptions)
    {
        _repository = repository;
        _query = query;
        _enrollmentQuery = enrollmentQuery;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _paymentRepository = paymentRepository;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
        _logger = logger;
        _billingOptions = billingOptions.Value;
    }

#region cruds 

    public async Task<List<InvoiceResponseDto>> GetAllAsync()
    {
        var invoices = await _query.GetAllAsync();
        return invoices.Select(InvoiceMapper.ToResponse).ToList();
    }

    public async Task<InvoiceResponseDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _query.GetByIdAsync(id);
        if (invoice == null) return null;
        return InvoiceMapper.ToResponse(invoice);
    }

    public async Task<InvoiceResponseDto> CreateAsync(InvoiceCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;
        var invoice = InvoiceMapper.ToDomain(command);
        var created = await _repository.AddAsync(invoice);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Invoice),
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return InvoiceMapper.ToResponse(created);
    }

    public async Task<InvoiceResponseDto> UpdateAsync(Guid id, UpdateInvoiceCommand command)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new NotFoundException($"No invoice found with id {id}");

        var oldValues = CreateAuditSnapshot(existing);
        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Invoice),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return InvoiceMapper.ToResponse(updated);
    }


     public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existing != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Invoice),
                entityId: existing.Id,
                branchId: _currentUserContext.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }
    }

#endregion cruds



    public async Task<InvoiceResponseDto> WaiveInvoiceAsync(Guid id, WaiveInvoiceCommand command)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice == null)
            throw new NotFoundException($"No invoice found with id {id}");

        var oldValues = CreateAuditSnapshot(invoice);

        invoice.WaiveInvoice(command.WaivedAmount, command.Reason);

        var updated = await _repository.UpdateAsync(invoice);

        await _auditLogService.StoreAsync(
            action: AuditLog.WaiveAction(),
            entityName: nameof(Invoice),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated),
            message: $"Waived amount {command.WaivedAmount} on invoice {updated.Id} for reason: {command.Reason}");

        return InvoiceMapper.ToResponse(updated);
    }

    public async Task<InvoiceResponseDto> CancelInvoiceAsync(Guid id, CancelInvoiceCommand command)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice == null)
            throw new NotFoundException($"No invoice found with id {id}");

        var oldValues = CreateAuditSnapshot(invoice);

        invoice.CancelInvoice(command.Reason);

        var updated = await _repository.UpdateAsync(invoice);

        if (updated.CreditAppliedAmount > 0)
        {
            var restoreAmount = _billingOptions.CalculateCreditRestoreAmount(
                updated.CreditAppliedAmount,
                updated.PeriodStart,
                DateTime.UtcNow);

            if (restoreAmount > 0)
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(updated.EnrollmentId);
                if (enrollment != null)
                {
                    enrollment.Student.AddCredit(restoreAmount);
                    await _enrollmentRepository.UpdateAsync(enrollment);

                    _logger.LogInformation(
                        "Restored {RestoreAmount} credit ({Percentage}%) to enrollment {EnrollmentId} after cancelling invoice {InvoiceId}.",
                        restoreAmount,
                        _billingOptions.CreditRestorePercentage,
                        enrollment.Id,
                        updated.Id);
                }
            }
            else
            {
                _logger.LogInformation(
                    "No credit restored for cancelled invoice {InvoiceId} (CreditApplied={CreditApplied}, restore rules not met).",
                    updated.Id,
                    updated.CreditAppliedAmount);
            }
        }

        await _auditLogService.StoreAsync(
            action: AuditLog.CancelAction(),
            entityName: nameof(Invoice),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated),
            message: $"Cancelled invoice {updated.Id} for reason: {command.Reason}");

        return InvoiceMapper.ToResponse(updated);
    }

   


    public async Task ProcessPastDueInvoicesAsync()
    {
        var pastDueInvoices = await _query.GetPastDueInvoicesAsync();
        if (pastDueInvoices.Count == 0)
        {
            _logger.LogInformation("[Hangfire] No past due invoices found to process.");
            return;
        }

        int processedCount = 0;
        foreach (var invoice in pastDueInvoices)
        {
            var oldStatus = invoice.Status;
            invoice.RecalculateStatus();

            if (invoice.Status == InvoiceStatus.PastDue)
            {
                await _repository.UpdateAsync(invoice);
                processedCount++;
                _logger.LogInformation("[Hangfire] Invoice {InvoiceId} status updated from {OldStatus} to PastDue.", invoice.Id, oldStatus);
            }
        }

        _logger.LogInformation("[Hangfire] ProcessPastDueInvoices completed. Total updated: {Count}.", processedCount);
    }

    public async Task GenerateDailyInvoicesAsync()
    {
        var expiringInvoices = await _query.GetInvoicesEndingWithinDaysAsync(days: 3);
        int generatedCount = 0;

        foreach (var expiringInvoice in expiringInvoices)
        {
            var enrollment = expiringInvoice.Enrollment;
            if (enrollment == null)
            {
                _logger.LogWarning(
                    "[Hangfire] Skipping invoice {InvoiceId}: enrollment not found.",
                    expiringInvoice.Id);
                continue;
            }

            var plan = enrollment.GetLatestPlan();
            if (plan == null)
            {
                _logger.LogWarning(
                    "[Hangfire] Skipping renewal for invoice {InvoiceId}, enrollment {EnrollmentId}: no plan assigned.",
                    expiringInvoice.Id,
                    enrollment.Id);
                continue;
            }

            decimal chargeAmount = plan.Amount;
            decimal creditApplied = 0;

            if (_billingOptions.ApplyCreditOnRenewalOnly && enrollment.Student.CreditBalance > 0)
            {
                creditApplied = Math.Min(enrollment.Student.CreditBalance, plan.Amount);
                chargeAmount = plan.Amount - creditApplied;
                enrollment.Student.UpdateCreditBalance(enrollment.Student.CreditBalance - creditApplied);
                await _studentRepository.UpdateAsync(enrollment.Student);
            }

            var nextPeriodStart = expiringInvoice.PeriodEnd.AddDays(1);
            var nextPeriodEnd = nextPeriodStart.AddMonths(plan.DurationMonths);
            var dueDate = nextPeriodStart.AddDays(plan.RemainingAmountDueDays);

            if (await _query.HasRenewalInvoiceAsync(expiringInvoice.EnrollmentId, expiringInvoice.PeriodEnd))
                continue;

            var newInvoice = Invoice.Create(
                enrollmentId: expiringInvoice.EnrollmentId,
                periodStart: nextPeriodStart,
                periodEnd: nextPeriodEnd,
                dueDate: dueDate,
                branchId: expiringInvoice.BranchId
            );

            if (creditApplied > 0)
                newInvoice.RecordCreditApplied(creditApplied);

            if (chargeAmount > 0)
            {
                var charge = Charge.Create(
                    invoiceId: newInvoice.Id,
                    amount: chargeAmount,
                    dueDate: dueDate
                );
                newInvoice.AddCharge(charge);
            }

            var created = await _repository.AddAsync(newInvoice);
            generatedCount++;

            _logger.LogInformation(
                "[Hangfire] Next-period invoice created: InvoiceId {InvoiceId} for EnrollmentId {EnrollmentId} (previous period ending {PeriodEnd}).",
                created.Id,
                expiringInvoice.EnrollmentId,
                expiringInvoice.PeriodEnd);
        }

        _logger.LogInformation("[Hangfire] GenerateDailyInvoices completed. Total new invoices created: {Count}.", generatedCount);
    }

    private static object CreateAuditSnapshot(Invoice invoice)
    {
        return new
        {
            invoice.Id,
            invoice.EnrollmentId,
            invoice.PeriodStart,
            invoice.PeriodEnd,
            invoice.DueDate,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.CreditAppliedAmount,
            invoice.Status,
            invoice.BranchId,
            Charge = invoice.Charge == null ? null : new
            {
                invoice.Charge.Id,
                invoice.Charge.Amount,
                invoice.Charge.PaidAmount,
                invoice.Charge.WaivedAmount,
                invoice.Charge.WaivedReason,
                invoice.Charge.Status
            }
        };
    }


    public async Task<PaymentResponseDto> RecordPaymentAsync(RecordInvoicePaymentCommand command)
    {
        var invoice = await _repository.GetByIdAsync(command.InvoiceId)
            ?? throw new NotFoundException($"Invoice {command.InvoiceId} not found");

        if (invoice.Status == Domain.Core.Enums.InvoiceStatus.Cancelled)
            throw new DomainException("Cannot record payment for a cancelled invoice");

        // Populate command context (computed attributes)
        command.ReceivedByStaffId = _currentUserContext.NameIdentifier;
        command.BranchId = invoice.BranchId;

        // Create payment record
        var payment = Payment.Create(
            enrollmentId: invoice.EnrollmentId,
            amount: command.Amount,
            status: Domain.Core.Enums.PaymentStatus.Completed,
            paidAt: command.PaidAt,
            branchId: command.BranchId,
            receivedByStaffId: command.ReceivedByStaffId,
            invoiceId: command.InvoiceId,
            transferFees: command.TransferFees,
            method: command.Method,
            externalReferenceCode: command.ExternalReferenceCode,
            methodDetailsJson: command.MethodDetailsJson
        );

        // Apply payment to invoice (updates invoice.PaidAmount and status)
        var result = invoice.AddPayment(command.Amount);

        // Save payment and updated invoice
        await _paymentRepository.AddAsync(payment);
        await _repository.UpdateAsync(invoice);

        // Audit log
        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Payment),
            entityId: payment.Id,
            branchId: command.BranchId,
            newValues: new
            {
                payment.Id,
                payment.Amount,
                payment.Method,
                payment.InvoiceId,
                payment.EnrollmentId,
                payment.PaidAt,
                payment.ReceivedByStaffId
            },
            message: $"Payment of {command.Amount:C} recorded for invoice {command.InvoiceId}");

        // If there was overpayment, it's handled by domain event (InvoiceOverpaymentDomainEvent)
        if (result.OverpaymentAmount > 0)
        {
            _logger.LogInformation(
                "Overpayment of {OverpaymentAmount:C} recorded for invoice {InvoiceId}. Applied: {AppliedAmount:C}",
                result.OverpaymentAmount, command.InvoiceId, result.AppliedAmount);
        }

        return PaymentMapper.ToResponse(payment);
    }
}
