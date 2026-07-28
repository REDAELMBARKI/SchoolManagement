using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.Services.Charges;

public class ChargeService : IChargeService
{
    private readonly IChargeRepository _repository;
    private readonly IAuditLogService _auditLogService;

    public ChargeService(IChargeRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    public async Task<List<ChargeResponseDto>> GetAllAsync()
    {
         throw new NotImplementedException();
    }

    public async Task<ChargeResponseDto?> GetByIdAsync(Guid id)
    {
        var charge = await _repository.GetByIdAsync(id);
        if (charge == null) return null;
        return ChargeMapper.ToResponse(charge);
    }

    public async Task<ChargeResponseDto> CreateAsync(ChargeCommand chargeCommand)
    {
        var charge = ChargeMapper.ToDomain(chargeCommand);
        var createdCharge = await _repository.AddAsync(charge);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Charge),
            entityId: createdCharge.Id,
            branchId: createdCharge.BranchId,
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
        existing.UpdateDescription(command.Description);
        existing.UpdateDueDate(command.DueDate);
        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Charge),
            entityId: updated.Id,
            branchId: updated.BranchId,
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
                branchId: existing.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }
    }

    private static object CreateAuditSnapshot(Charge charge)
    {
        return new
        {
            charge.Id,
            charge.StudentId,
            charge.ChargeType,
            charge.Description,
            charge.Amount,
            charge.AmountPaid,
            charge.Status,
            charge.IssuedDate,
            charge.DueDate,
            charge.SourceId,
            charge.BranchId,
            charge.CurrencyCode
        };
    }
}
