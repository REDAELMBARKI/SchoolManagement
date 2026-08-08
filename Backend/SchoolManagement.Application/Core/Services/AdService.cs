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

public class AdService : IAdService
{
    private readonly IAdRepository _repository;
    private readonly IAdQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public AdService(
        IAdRepository repository,
        IAdQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<AdResponseDto>> GetAllAsync()
    {
        var ads = await _query.GetAllAsync();
        return ads.Select(AdMapper.ToResponse).ToList();
    }

    public async Task<AdResponseDto?> GetByIdAsync(Guid id)
    {
        var ad = await _query.GetByIdAsync(id);
        if (ad == null) return null;
        return AdMapper.ToResponse(ad);
    }

    public async Task<AdResponseDto> CreateAsync(AdCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;

        // Generate unique slug from Name + PlatformId
        var baseSlug = $"{command.Name}-{command.PlatformId}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        var ad = AdMapper.ToDomain(command);
        var created = await _repository.AddAsync(ad);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Ad),
            entityId: created.Id,
            branchId: branchId,
            newValues: CreateAuditSnapshot(created),
            message: $"Ad '{created.Name}' created.");

        return AdMapper.ToResponse(created);
    }

    public async Task<AdResponseDto> UpdateAsync(Guid id, UpdateAdCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        var existing = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No ad found with id {id}");

        var oldValues = CreateAuditSnapshot(existing);

        // Generate unique slug if name or platform changed
        if (existing.Name != command.Name || existing.PlatformId != command.PlatformId)
        {
            var baseSlug = $"{command.Name}-{command.PlatformId}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
            existing.UpdateSlug(command.Slug);
        }

        existing.UpdateName(command.Name);
        existing.UpdatePlatformId(command.PlatformId);
        existing.UpdateBranchId(command.BranchId);

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Ad),
            entityId: updated.Id,
            branchId: branchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return AdMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var branchId = _currentUserContext.BranchId;
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return;

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: nameof(Ad),
            entityId: existing.Id,
            branchId: branchId,
            oldValues: CreateAuditSnapshot(existing));
    }

    private static object CreateAuditSnapshot(Ad ad) => new
    {
        ad.Id,
        ad.Name,
        ad.Slug,
        ad.PlatformId,
        ad.BranchId
    };
}
