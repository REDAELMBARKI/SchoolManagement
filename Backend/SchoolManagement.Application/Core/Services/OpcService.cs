using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Utils;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class OpcService : IOpcService
{
    private readonly IOpcRepository _repository;
    private readonly IOpcQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public OpcService(
        IOpcRepository repository,
        IOpcQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<OpcResponseDto>> GetAllAsync()
    {
        var opcs = await _query.GetAllAsync();
        return opcs.Select(OpcMapper.ToResponse).ToList();
    }

    public async Task<OpcResponseDto?> GetByIdAsync(Guid id)
    {
        var opc = await _query.GetByIdAsync(id);
        if (opc == null) return null;
        return OpcMapper.ToResponse(opc);
    }

    public async Task<OpcResponseDto> CreateAsync(OpcCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;

        // Generate unique slug from FirstName + LastName + Phone
        var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        var opc = OpcMapper.ToDomain(command);
        var created = await _repository.AddAsync(opc);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Opc),
            entityId: created.Id,
            branchId: branchId,
            newValues: CreateAuditSnapshot(created),
            message: $"OPC '{created.FirstName} {created.LastName}' registered.");

        return OpcMapper.ToResponse(created);
    }

    public async Task<OpcResponseDto> UpdateAsync(Guid id, UpdateOpcCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        var existing = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No OPC found with id {id}");

        var oldValues = CreateAuditSnapshot(existing);

        // Check if name or phone changed - regenerate slug if needed
        bool nameOrPhoneChanged = existing.FirstName != command.FirstName || 
                                   existing.LastName != command.LastName || 
                                   existing.Phone != command.Phone;

        if (nameOrPhoneChanged)
        {
            var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
        }

        existing.UpdateFirstName(command.FirstName);
        existing.UpdateLastName(command.LastName);
        existing.UpdateSlug(command.Slug);
        existing.UpdateGenderId(command.GenderId);
        existing.UpdateEmail(command.Email);
        existing.UpdatePhone(command.Phone);
        existing.UpdateDateOfBirth(command.DateOfBirth);
        existing.UpdateHireDate(command.HireDate);
        existing.UpdateSalary(command.Salary);
        existing.UpdateBranchId(command.BranchId);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Opc),
            entityId: updated.Id,
            branchId: branchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return OpcMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var branchId = _currentUserContext.BranchId;
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return;

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: nameof(Opc),
            entityId: existing.Id,
            branchId: branchId,
            oldValues: CreateAuditSnapshot(existing));
    }

    private static object CreateAuditSnapshot(Opc opc) => new
    {
        opc.Id,
        opc.FirstName,
        opc.LastName,
        opc.Slug,
        opc.GenderId,
        Email = opc.Email?.Value,
        opc.Phone,
        opc.DateOfBirth,
        opc.HireDate,
        opc.Salary,
        opc.BranchId
    };
}
