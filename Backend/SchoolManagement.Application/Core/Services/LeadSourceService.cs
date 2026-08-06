using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class LeadSourceService : ILeadSourceService
{
    private readonly ILeadSourceRepository _repository;
    private readonly ILeadSourceQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public LeadSourceService(
        ILeadSourceRepository repository,
        ILeadSourceQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<List<LeadSourceResponseDto>> GetAllAsync()
    {
        var leadSources = await _query.GetAllAsync();
        return leadSources.Select(LeadSourceMapper.ToResponse).ToList();
    }

    public async Task<LeadSourceResponseDto?> GetByIdAsync(Guid id)
    {
        var leadSource = await _query.GetByIdAsync(id);
        if (leadSource == null) return null;
        return LeadSourceMapper.ToResponse(leadSource);
    }

    public async Task<LeadSourceResponseDto> CreateAdLeadSourceAsync(AdLeadSourceCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;

        var leadSource = LeadSourceMapper.ToDomain(command);
        var created = await _repository.AddAsync(leadSource);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(AdLeadSource),
            entityId: created.Id,
            branchId: branchId,
            newValues: CreateAuditSnapshot(created),
            message: $"Ad LeadSource created for Ad ID: {command.AdId}");

        return LeadSourceMapper.ToResponse(created);
    }

    public async Task<LeadSourceResponseDto> CreateOpcLeadSourceAsync(OpcLeadSourceCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        command.BranchId = branchId;

        var leadSource = LeadSourceMapper.ToDomain(command);
        var created = await _repository.AddAsync(leadSource);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(OpcLeadSource),
            entityId: created.Id,
            branchId: branchId,
            newValues: CreateAuditSnapshot(created),
            message: $"Opc LeadSource created for Opc ID: {command.OpcId}");

        return LeadSourceMapper.ToResponse(created);
    }

    public async Task DeleteAsync(Guid id)
    {
        var branchId = _currentUserContext.BranchId;
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return;

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: nameof(LeadSource),
            entityId: existing.Id,
            branchId: branchId,
            oldValues: CreateAuditSnapshot(existing));
    }

    private static object CreateAuditSnapshot(LeadSource leadSource)
    {
        return leadSource switch
        {
            AdLeadSource adLead => new
            {
                adLead.Id,
                adLead.BranchId,
                Type = "Ad",
                adLead.AdId
            },
            OpcLeadSource opcLead => new
            {
                opcLead.Id,
                opcLead.BranchId,
                Type = "Opc",
                opcLead.OpcId
            },
            _ => new
            {
                leadSource.Id,
                leadSource.BranchId,
                Type = "Unknown"
            }
        };
    }
}
