using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Common.Services;

public class GenderService : IGenderService
{
    private readonly IGenderRepository _repository;
    private readonly IGenderQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public GenderService(
        IGenderRepository repository,
        IGenderQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<GenderResponseDto>> GetAllAsync()
    {
        var genders = await _query.GetAllAsync();
        return genders.Select(GenderMapper.ToResponse).ToList();
    }

    public async Task<GenderResponseDto?> GetByIdAsync(Guid id)
    {
        var gender = await _query.GetByIdAsync(id);
        if (gender == null) return null;
        return GenderMapper.ToResponse(gender);
    }

    public async Task<GenderResponseDto> CreateAsync(GenderCommand command)
    {
        var gender = GenderMapper.ToDomain(command);
        var created = await _repository.AddAsync(gender);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Gender),
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created),
            message: $"Gender '{created.Name}' created.");

        return GenderMapper.ToResponse(created);
    }

    public async Task<GenderResponseDto> UpdateAsync(Guid id, UpdateGenderCommand command)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No gender found with id {id}");

        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateName(command.Name);
        existing.UpdateSlug(command.Slug);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Gender),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return GenderMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return;

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: nameof(Gender),
            entityId: existing.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: CreateAuditSnapshot(existing));
    }

    private static object CreateAuditSnapshot(Gender gender) => new
    {
        gender.Id,
        gender.Name,
        gender.Slug
    };
}
