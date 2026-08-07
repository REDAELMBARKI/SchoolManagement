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

public class GradeService : IGradeService
{
    private readonly IGradeRepository _repository;
    private readonly IGradeQueryService _queryService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public GradeService(
        IGradeRepository repository,
        IGradeQueryService queryService,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _queryService = queryService;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<GradeResponseDto> CreateAsync(GradeCommand command)
    {
        var grade = GradeMapper.ToDomain(command);

        // Use repository for tracking operations
        await _repository.AddAsync(grade);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Grade",
            entityId: grade.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(grade));

        return GradeMapper.ToResponse(grade);
    }

    public async Task<GradeResponseDto> UpdateAsync(Guid id, UpdateGradeCommand command)
    {
        // Use repository for tracking operations
        var grade = await _repository.GetByIdAsync(id);
        if (grade == null)
        {
            throw new NotFoundException($"Grade with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(grade);

        grade.UpdateEvaluationType(command.EvaluationType);
        grade.UpdateScore(command.Score);
        grade.UpdateMaxScore(command.MaxScore);
        grade.UpdateComment(command.Comment);

        await _repository.UpdateAsync(grade);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Grade",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(grade));

        return GradeMapper.ToResponse(grade);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations
        var grade = await _repository.GetByIdAsync(id);
        if (grade == null)
        {
            throw new NotFoundException($"Grade with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(grade);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Grade",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<GradeResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var grade = await _queryService.GetResponseByIdAsync(id);
        if (grade == null)
        {
            throw new NotFoundException($"Grade with ID {id} not found.");
        }

        return grade;
    }

    public async Task<List<GradeResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<List<GradeResponseDto>> GetByStudentAsync(Guid studentId)
    {
        // Use query service for non-tracking read operations
        var grades = await _queryService.GetByStudentIdAsync(studentId);
        return grades.Select(GradeMapper.ToResponse).ToList();
    }

    public async Task<List<GradeResponseDto>> GetByGroupTeacherAsync(Guid groupTeacherId)
    {
        // Use query service for non-tracking read operations
        var grades = await _queryService.GetByGroupTeacherIdAsync(groupTeacherId);
        return grades.Select(GradeMapper.ToResponse).ToList();
    }

    private static object CreateAuditSnapshot(Grade grade)
    {
        return new
        {
            grade.Id,
            grade.EvaluationType,
            grade.Score,
            grade.MaxScore,
            grade.EvaluationDate,
            grade.Comment,
            grade.StudentId,
            grade.GroupTeacherId,
            grade.BranchId
        };
    }
}
