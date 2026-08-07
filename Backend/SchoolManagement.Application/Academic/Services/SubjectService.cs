using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Application.Academic.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repository;
    private readonly ISubjectQueryService _queryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public SubjectService(
        ISubjectRepository repository,
        ISubjectQueryService queryService,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _queryService = queryService;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<SubjectResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<SubjectResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var subject = await _queryService.GetResponseByIdAsync(id);
        if (subject == null)
        {
            throw new NotFoundException($"Subject with ID {id} not found.");
        }
        return subject;
    }

    public async Task<SubjectResponseDto> CreateAsync(SubjectCommand command)
    {
        var subject = SubjectMapper.ToDomain(command, _currentUserContext.BranchId);
        
        // Use repository for tracking operations
        var created = await _repository.AddAsync(subject);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Subject",
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return SubjectMapper.ToResponse(created);
    }

    public async Task<SubjectResponseDto> UpdateAsync(Guid id, UpdateSubjectCommand command)
    {
        // Use repository for tracking operations
        var subject = await _repository.GetByIdAsync(id);
        if (subject == null)
        {
            throw new NotFoundException($"Subject with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(subject);

        subject.UpdateName(command.Name);
        subject.UpdateDescription(command.Description);

        var updated = await _repository.UpdateAsync(subject);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Subject",
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return SubjectMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations
        var subject = await _repository.GetByIdAsync(id);
        if (subject == null)
        {
            throw new NotFoundException($"Subject with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(subject);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Subject",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    private static object CreateAuditSnapshot(Domain.Academic.Entities.Subject subject)
    {
        return new
        {
            subject.Id,
            subject.Name,
            subject.Slug,
            subject.Description,
            subject.BranchId
        };
    }
}
