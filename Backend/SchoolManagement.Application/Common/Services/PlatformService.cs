using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Common.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public PlatformService(
        IPlatformRepository repository,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<PlatformResponseDto> CreateAsync(PlatformCommand command)
    {
        var platform = PlatformMapper.ToDomain(command);

        await _repository.AddAsync(platform);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Platform",
            entityId: platform.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(platform));

        return PlatformMapper.ToResponse(platform);
    }

    public async Task<PlatformResponseDto> UpdateAsync(Guid id, UpdatePlatformCommand command)
    {
        var platform = await _repository.GetByIdAsync(id);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(platform);

        platform.UpdateName(command.Name);
        platform.UpdateSlug(command.Slug);

        await _repository.UpdateAsync(platform);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Platform",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(platform));

        return PlatformMapper.ToResponse(platform);
    }

    public async Task DeleteAsync(Guid id)
    {
        var platform = await _repository.GetByIdAsync(id);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(platform);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Platform",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<PlatformResponseDto> GetByIdAsync(Guid id)
    {
        var platform = await _repository.GetByIdAsync(id);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with ID {id} not found.");
        }

        return PlatformMapper.ToResponse(platform);
    }

    public async Task<List<PlatformResponseDto>> GetAllAsync()
    {
        var platforms = await _repository.GetAllAsync();
        return platforms.Select(PlatformMapper.ToResponse).ToList();
    }

    private static object CreateAuditSnapshot(Platform platform)
    {
        return new
        {
            platform.Id,
            platform.Name,
            platform.Slug,
            platform.BranchId
        };
    }
}
