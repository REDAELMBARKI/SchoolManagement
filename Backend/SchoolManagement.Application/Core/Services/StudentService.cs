using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IStudentQueryService _query;
    private readonly IMediator _mediator;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IStudentResponsableRepository _responsableRepository;

    public StudentService(
        IStudentRepository repository,
        IStudentQueryService query,
        IMediator mediator,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext,
        IStudentResponsableRepository responsableRepository)
    {
        _repository = repository;
        _query = query;
        _mediator = mediator;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
        _responsableRepository = responsableRepository;
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

    public async Task<StudentResponseDto> TransferBranchAsync(Guid studentId, TransferBranchCommand command)
    {
        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new NotFoundException($"No student found with id {studentId}");
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new DomainException("Transfer reason is required.");
        }

        if (command.NewBranchId == Guid.Empty)
        {
            throw new DomainException("New branch ID must not be empty.");
        }

        if (student.BranchId == command.NewBranchId)
        {
            throw new DomainException("Student is already in this branch.");
        }

        var oldBranchId = student.BranchId;
        var oldValues = CreateAuditSnapshot(student);

        student.UpdateBranchId(command.NewBranchId);
        var updated = await _repository.UpdateAsync(student);

        await _auditLogService.StoreAsync(
            action: "TransferBranch",
            entityName: nameof(Student),
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated),
            additionalInfo: new
            {
                OldBranchId = oldBranchId,
                NewBranchId = command.NewBranchId,
                Reason = command.Reason
            });

        return StudentMapper.ToResponse(updated);
    }

    public async Task<List<StudentResponsableResponseDto>> GetParentsByStudentIdAsync(Guid studentId)
    {
        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new NotFoundException($"No student found with id {studentId}");
        }

        return student.StudentResponsables
            .Select(sr => new StudentResponsableResponseDto
            {
                Id = sr.Id,
                FirstName = sr.FirstName,
                LastName = sr.LastName,
                Email = sr.Email,
                Phone = sr.Phone,
                Relationship = sr.Relationship.ToString()
            })
            .ToList();
    }

    public async Task<StudentResponsableResponseDto> AddParentToStudentAsync(Guid studentId, StudentResponsableRequestDto request)
    {
        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new NotFoundException($"No student found with id {studentId}");
        }

        // Generate slug for parent/guardian
        var responsableSlug = await CustomSluger.Slug(
            slug => _responsableRepository.IsExistsBySlugAsync(slug),
            $"{request.FirstName}-{request.LastName}"
        );

        // Create StudentResponsable entity
        var responsable = StudentResponsable.Register(
            firstName: request.FirstName,
            lastName: request.LastName,
            slug: responsableSlug,
            genderId: request.GenderId,
            email: request.Email,
            phone: request.Phone,
            relationship: request.Relationship,
            branchId: _currentUserContext.BranchId
        );

        var createdResponsable = await _responsableRepository.AddAsync(responsable);

        // Link parent to student
        student.StudentResponsables.Add(createdResponsable);
        await _repository.UpdateAsync(student);

        // Audit log
        await _auditLogService.StoreAsync(
            action: "AddParent",
            entityName: nameof(StudentResponsable),
            entityId: createdResponsable.Id,
            branchId: _currentUserContext.BranchId,
            newValues: new
            {
                createdResponsable.Id,
                createdResponsable.FirstName,
                createdResponsable.LastName,
                createdResponsable.Email,
                createdResponsable.Phone,
                createdResponsable.Relationship,
                LinkedStudentId = studentId
            });

        return new StudentResponsableResponseDto
        {
            Id = createdResponsable.Id,
            FirstName = createdResponsable.FirstName,
            LastName = createdResponsable.LastName,
            Email = createdResponsable.Email,
            Phone = createdResponsable.Phone,
            Relationship = createdResponsable.Relationship.ToString()
        };
    }

    public async Task RemoveParentFromStudentAsync(Guid studentId, Guid parentId)
    {
        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new NotFoundException($"No student found with id {studentId}");
        }

        var parent = student.StudentResponsables.FirstOrDefault(sr => sr.Id == parentId);
        if (parent == null)
        {
            throw new NotFoundException($"No parent found with id {parentId} for student {studentId}");
        }

        student.StudentResponsables.Remove(parent);
        await _repository.UpdateAsync(student);

        await _auditLogService.StoreAsync(
            action: "RemoveParent",
            entityName: nameof(StudentResponsable),
            entityId: parentId,
            branchId: _currentUserContext.BranchId,
            oldValues: new
            {
                parent.Id,
                parent.FirstName,
                parent.LastName,
                parent.Email,
                parent.Phone,
                parent.Relationship,
                UnlinkedStudentId = studentId
            });
    }
}
