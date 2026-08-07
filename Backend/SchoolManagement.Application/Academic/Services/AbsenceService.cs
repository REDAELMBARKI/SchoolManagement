using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Application.Academic.Services;

public class AbsenceService : IAbsenceService
{
    private readonly IAbsenceRepository _repository;
    private readonly IAbsenceQueryService _queryService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public AbsenceService(
        IAbsenceRepository repository,
        IAbsenceQueryService queryService,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _queryService = queryService;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<AbsenceResponseDto> CreateAsync(AbsenceCommand command)
    {
        var absence = AbsenceMapper.ToDomain(command);

        // Use repository for tracking operations
        await _repository.AddAsync(absence);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Absence",
            entityId: absence.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(absence));

        return AbsenceMapper.ToResponse(absence);
    }

    public async Task<AbsenceResponseDto> UpdateAsync(Guid id, UpdateAbsenceCommand command)
    {
        // Use repository for tracking operations
        var absence = await _repository.GetByIdAsync(id);
        if (absence == null)
        {
            throw new NotFoundException($"Absence with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(absence);

        absence.UpdateStatus(command.Status);
        absence.UpdateIsJustified(command.IsJustified);
        absence.UpdateReason(command.Reason);

        await _repository.UpdateAsync(absence);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Absence",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(absence));

        return AbsenceMapper.ToResponse(absence);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations
        var absence = await _repository.GetByIdAsync(id);
        if (absence == null)
        {
            throw new NotFoundException($"Absence with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(absence);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Absence",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<AbsenceResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var absence = await _queryService.GetResponseByIdAsync(id);
        if (absence == null)
        {
            throw new NotFoundException($"Absence with ID {id} not found.");
        }

        return absence;
    }

    public async Task<List<AbsenceResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<List<AbsenceResponseDto>> GetByStudentAsync(Guid studentId)
    {
        // Use query service for non-tracking read operations
        var absences = await _queryService.GetByStudentIdAsync(studentId);
        return absences.Select(AbsenceMapper.ToResponse).ToList();
    }

    public async Task<List<AbsenceResponseDto>> GetByScheduleAsync(Guid scheduleId)
    {
        // Use query service for non-tracking read operations
        var absences = await _queryService.GetByScheduleIdAsync(scheduleId);
        return absences.Select(AbsenceMapper.ToResponse).ToList();
    }

    private static object CreateAuditSnapshot(Absence absence)
    {
        return new
        {
            absence.Id,
            absence.StudentId,
            absence.ScheduleId,
            absence.BranchId,
            absence.Date,
            absence.Status,
            absence.IsJustified,
            absence.Reason
        };
    }
}
