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

public class CommercialAgentService : ICommercialAgentService
{
    private readonly ICommercialAgentRepository _repository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICommercialAgentQueryService _query;

    public CommercialAgentService(
        ICommercialAgentRepository repository,
        IAuditLogService auditLogService,
        ICommercialAgentQueryService query,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _query = query;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<CommercialAgentResponseDto> CreateAsync(CommercialAgentCommand command)
    {
        // Generate unique slug from FirstName + LastName + Phone
        var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        var agent = CommercialAgentMapper.ToDomain(command);

        await _repository.AddAsync(agent);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "CommercialAgent",
            entityId: agent.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(agent));

        return CommercialAgentMapper.ToResponse(agent);
    }

    public async Task<CommercialAgentResponseDto> UpdateAsync(Guid id, UpdateCommercialAgentCommand command)
    {
        var agent = await _repository.GetByIdAsync(id);
        if (agent == null)
        {
            throw new NotFoundException($"CommercialAgent with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(agent);

        // Generate unique slug if name or phone changed
        if (agent.FirstName != command.FirstName || agent.LastName != command.LastName || agent.Phone != command.Phone)
        {
            var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
            agent.UpdateSlug(command.Slug);
        }

        // Replace non-existing UpdatePersonalInfo with existing methods
        agent.UpdateFirstName(command.FirstName);
        agent.UpdateLastName(command.LastName);
        agent.UpdateEmail(command.Email);
        agent.UpdatePhone(command.Phone);

        agent.UpdateSalary(command.Salary);

        await _repository.UpdateAsync(agent);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "CommercialAgent",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(agent));

        return CommercialAgentMapper.ToResponse(agent);
    }

    public async Task DeleteAsync(Guid id)
    {
        var agent = await _repository.GetByIdAsync(id);
        if (agent == null)
        {
            throw new NotFoundException($"CommercialAgent with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(agent);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "CommercialAgent",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<CommercialAgentResponseDto> GetByIdAsync(Guid id)
    {
        var agent = await _repository.GetByIdAsync(id);
        if (agent == null)
        {
            throw new NotFoundException($"CommercialAgent with ID {id} not found.");
        }

        return CommercialAgentMapper.ToResponse(agent);
    }

    public async Task<List<CommercialAgentResponseDto>> GetAllAsync()
    {
        var agents = await _query.GetAllAsync();
        return agents.Select(CommercialAgentMapper.ToResponse).ToList();
    }

 
    private static object CreateAuditSnapshot(CommercialAgent agent)
    {
        return new
        {
            agent.Id,
            agent.FirstName,
            agent.LastName,
            agent.Slug,
            agent.GenderId,
            agent.Email,
            agent.Phone,
            agent.DateOfBirth,
            agent.HireDate,
            agent.Salary,
            agent.BranchId
        };
    }
}
