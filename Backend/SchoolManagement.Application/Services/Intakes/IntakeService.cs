using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Utils;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Services.Intakes;

public class IntakeService : IIntakeService
{
    private readonly IIntakeRepository _repository;
    private readonly IIntakeQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public IntakeService(
        IIntakeRepository repository,
        IIntakeQueryService query,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync()
    {
        return await _query.GetAllResponsesAsync();
    }

    public async Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id)
    {
        return await _query.GetResponseByIdAsync(id);
    }

    public async Task<IntakeResponseDto> AddIntakeAsync(IntakeCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var generatedSlug = await CustomSluger.Slug(slug => _query.IsExistsBySlugAsync(slug), $"{command.FirstName}-{command.LastName}");
        command.Slug = generatedSlug;

        Intake intake = IntakeMapper.ToDomain(command);
        var newEntity = await _repository.AddAsync(intake);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Intake),
            entityId: newEntity.Id,
            branchId: newEntity.BranchId,
            newValues: CreateAuditSnapshot(newEntity));

        return IntakeMapper.ToResponse(newEntity);
    }


   
    
    public async Task<IntakeResponseDto?> UpdateAsync(Guid id, UpdateIntakeCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existingIntake = await _repository.GetByIdAsync(id);
        if (existingIntake is null)
        {
            throw new NotFoundException($"No intake found with id {id}");
        }
        
        if (existingIntake.HasStudents)
        {
            throw new DomainException("Cannot update an intake that already has students.");
        }
        
        var generatedSlug = await CustomSluger.Slug(slug => _query.IsExistsBySlugAsync(slug), $"{command.FirstName}-{command.LastName}");
        var oldValues = CreateAuditSnapshot(existingIntake);


        existingIntake.UpdateFirstName(command.FirstName);
        existingIntake.UpdateLastName(command.LastName);
        existingIntake.UpdateSlug(generatedSlug);
        existingIntake.UpdateGenderId(command.GenderId);
        existingIntake.UpdatePhone(command.Phone);
        existingIntake.UpdateEmail(command.Email);
        existingIntake.UpdateDateOfBirth(command.DateOfBirth);
        existingIntake.UpdateIntakeDate(command.IntakeDate);
        existingIntake.UpdateStatus(command.Status);
        existingIntake.UpdateFollowUpDate(command.FollowUpDate);
        existingIntake.UpdateNotes(command.Notes);
        existingIntake.UpdateCommercialAgentId(command.CommercialAgentId);
        existingIntake.UpdateLeadSourceId(command.LeadSourceId);
        existingIntake.UpdateSubjectId(command.SubjectId);
        existingIntake.UpdateBranchId(command.BranchId);
        existingIntake.UpdateIsIndependent(command.IsIndependent);
        existingIntake.UpdateTotalFees(command.TotalFees);
        existingIntake.UpdateAmountPaid(command.AmountPaid);
        
        existingIntake.UpdatedAt = DateTime.UtcNow;
        Intake  updatedIntake = await _repository.UpdateAsync(existingIntake);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Intake),
            entityId: updatedIntake.Id,
            branchId: updatedIntake.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updatedIntake));

        return IntakeMapper.ToResponse(updatedIntake); 
    }


    public async Task DeleteIntakeAsync(Guid id)
    {
        var existingIntake = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existingIntake != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Intake),
                entityId: existingIntake.Id,
                branchId: existingIntake.BranchId,
                oldValues: CreateAuditSnapshot(existingIntake));
        }
    }
 
    private static object CreateAuditSnapshot(Intake intake)
    {
        return new
        {
            intake.Id,
            intake.FirstName,
            intake.LastName,
            intake.Slug,
            Email = intake.Email?.Value,
            intake.Phone,
            intake.DateOfBirth,
            intake.GenderId,
            intake.IntakeDate,
            intake.Status,
            intake.FollowUpDate,
            intake.Notes,
            intake.CommercialAgentId,
            intake.LeadSourceId,
            intake.SubjectId,
            intake.BranchId,
            intake.IsIndependent,
            intake.TotalFees,
            intake.AmountPaid
        };
    }

}
