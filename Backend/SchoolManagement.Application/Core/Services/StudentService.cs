using MediatR;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Utils;
using Slugify;

namespace SchoolManagement.Application.Core.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IStudentQueryService _query;
    private readonly IMediator _mediator;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public StudentService(
        IStudentRepository repository,
        IStudentQueryService query,
        IMediator mediator,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _query = query;
        _mediator = mediator;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        return await _query.GetAllResponsesAsync();
    }

    public async Task<StudentResponseDto> GetByIdAsync(Guid id)
    {
        var student = await _query.GetResponseByIdAsync(id);
        if(student == null ) {
            throw new NotFoundException($"No student found with id {id}");
        }
        return student;
    }

    public async Task<StudentResponseDto> CreateAsync(StudentCommand command)
    {
        await EnsureNoDuplicateStudentAsync(command);

        var generatedSlug = await CustomSluger.Slug(
            slug => _query.IsExistsBySlugAsync(slug),
            $"{command.FirstName}-{command.LastName}"
        );

        command.Slug = generatedSlug;
        
        var student = StudentMapper.ToDomain(command);
        var createdStudent = await _repository.AddAsync(student);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: nameof(Student),
            entityId: createdStudent.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(createdStudent));
        
        await _mediator.Publish(new StudentCreatedDomainEvent(createdStudent.Id));
        
        return StudentMapper.ToResponse(createdStudent);
    }

    public async Task EnsureNoDuplicateStudentAsync(StudentCommand command)
    {
        if (await _query.HasDuplicateByPhoneAsync(command.Phone))
            throw new DomainException("A student with this phone number already exists.");

        if (!string.IsNullOrWhiteSpace(command.Email) && await _query.HasDuplicateByEmailAsync(command.Email))
            throw new DomainException("A student with this email already exists.");

        if (await _query.HasDuplicateByNameDobAsync(command.FirstName, command.LastName, command.DateOfBirth))
            throw new DomainException("A student with the same name and date of birth already exists.");
    }

    public async Task<StudentResponseDto> UpdateAsync(Guid id, UpdateStudentCommand command)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No student found with id {id}");
        }
        
        var oldValues = CreateAuditSnapshot(existing);

        existing.UpdateFirstName(command.FirstName);
        existing.UpdateLastName(command.LastName);
        existing.UpdateEmail(command.Email);
        existing.UpdatePhone(command.Phone);
        existing.UpdateDateOfBirth(command.DateOfBirth);
        existing.UpdateGenderId(command.GenderId);
        if (existing.IntakeId != command.IntakeId && existing.IsDirectRegistration != command.IsDirectRegistration)
        {
            existing.UpdateRegistrationSource(command.IntakeId, command.IsDirectRegistration);
        }
        else
        {
            existing.UpdateIntakeId(command.IntakeId);
            existing.UpdateIsDirectRegistration(command.IsDirectRegistration);
        }

        var updated = await _repository.UpdateAsync(existing);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: nameof(Student),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return StudentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(id);

        if (existing != null)
        {
            await _auditLogService.StoreAsync(
                action: AuditLog.DeleteAction(),
                entityName: nameof(Student),
                entityId: existing.Id,
                branchId: _currentUserContext.BranchId,
                oldValues: CreateAuditSnapshot(existing));
        }
    }

    private static object CreateAuditSnapshot(Student student)
    {
        return new
        {
            student.Id,
            student.FirstName,
            student.LastName,
            Email = student.Email?.Value,
            student.Phone,
            student.DateOfBirth,
            student.GenderId,
            student.IntakeId,
            student.IsDirectRegistration,
            student.BranchId,
            student.Slug
        };
    }
}
