using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class ChargeService : IChargeService
{
    private readonly IChargeRepository _repository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IChargeQueryService _queryService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public ChargeService(
        IChargeRepository repository,
        IInvoiceRepository invoiceRepository,
        IChargeQueryService queryService,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _invoiceRepository = invoiceRepository;
        _queryService = queryService;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<List<ChargeResponseDto>> GetAllAsync()
    {
        var charges = await _queryService.GetAllAsync();
        return charges.Select(ChargeMapper.ToResponse).ToList();
    }

    public async Task<ChargeResponseDto> GetByIdAsync(Guid id)
    {
        var charge = await _queryService.GetByIdAsync(id);
        if (charge == null)
        {
            throw new NotFoundException($"Charge with id '{id}' not found.");
        }
        return ChargeMapper.ToResponse(charge);
    }

    public async Task<ChargeResponseDto> CreateAsync(ChargeCommand chargeCommand)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(chargeCommand.InvoiceId);
        if (invoice == null)
            throw new NotFoundException($"No invoice found with id {chargeCommand.InvoiceId}");
        if (invoice.Charge != null)
            throw new DomainException("Only one charge is allowed per invoice.");

        var charge = ChargeMapper.ToDomain(chargeCommand);
        var createdCharge = await _repository.AddAsync(charge);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Charge),
            entityId: createdCharge.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(createdCharge));

        return ChargeMapper.ToResponse(createdCharge);
    }

    public async Task<ChargeResponseDto> UpdateAsync(Guid id, UpdateChargeCommand command)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No charge found with id {id}");
        }

        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateAmount(command.Amount);
        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Charge),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return ChargeMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existing != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Charge),
                entityId: existing.Id,
                branchId: _currentUserContext.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }
    }

    private static object CreateAuditSnapshot(Charge charge)
    {
        return new
        {
            charge.Id,
            charge.InvoiceId,
            charge.Amount,
            charge.DueDate
        };
    }
}
