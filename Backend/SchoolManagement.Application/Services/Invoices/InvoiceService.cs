using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.Services.Invoices;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IInvoiceQueryService _query;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository repository,
        IInvoiceQueryService query,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext,
        ILogger<InvoiceService> logger)
    {
        _repository = repository;
        _query = query;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
        _logger = logger;
    }

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

    public async Task<int> ProcessPastDueInvoicesAsync()
    {
        var pastDueInvoices = await _query.GetPastDueInvoicesAsync();
        if (pastDueInvoices.Count == 0)
        {
            _logger.LogInformation("[Hangfire] No past due invoices found to process.");
            return 0;
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
        return processedCount;
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
            invoice.Status,
            invoice.BranchId
        };
    }
}
