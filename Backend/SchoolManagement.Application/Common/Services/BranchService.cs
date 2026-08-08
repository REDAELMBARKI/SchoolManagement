using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Common.Utils;

namespace SchoolManagement.Application.Common.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _repository;
    private readonly IBranchQueryService _queryService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public BranchService(
        IBranchRepository repository,
        IBranchQueryService queryService,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _queryService = queryService;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<BranchResponseDto> CreateAsync(BranchCommand command)
    {
        // Generate unique slug from name + city
        var baseSlug = $"{command.Name}-{command.City}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        var branch = BranchMapper.ToDomain(command);

        await _repository.AddAsync(branch);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Branch",
            entityId: branch.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(branch));

        return BranchMapper.ToResponse(branch);
    }

    public async Task<BranchResponseDto> UpdateAsync(Guid id, UpdateBranchCommand command)
    {
        // Use repository for tracking operations
        var branch = await _repository.GetByIdAsync(id);
        if (branch == null)
        {
            throw new NotFoundException($"Branch with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(branch);

        // Generate unique slug if name or city changed
        if (branch.Name != command.Name || branch.City != command.City)
        {
            var baseSlug = $"{command.Name}-{command.City}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
            branch.UpdateSlug(command.Slug);
        }

        branch.UpdateName(command.Name);
        branch.UpdateCity(command.City);
        branch.UpdateAddress(command.Address);
        branch.UpdatePhone(command.Phone);

        await _repository.UpdateAsync(branch);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Branch",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(branch));

        return BranchMapper.ToResponse(branch);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations
        var branch = await _repository.GetByIdAsync(id);
        if (branch == null)
        {
            throw new NotFoundException($"Branch with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(branch);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Branch",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<BranchResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var branch = await _queryService.GetResponseByIdAsync(id);
        if (branch == null)
        {
            throw new NotFoundException($"Branch with ID {id} not found.");
        }

        return branch;
    }

    public async Task<List<BranchResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _queryService.GetAllResponsesAsync();
    }

    private static object CreateAuditSnapshot(Branch branch)
    {
        return new
        {
            branch.Id,
            branch.Name,
            branch.Slug,
            branch.City,
            branch.Address,
            branch.Phone
        };
    }
}
