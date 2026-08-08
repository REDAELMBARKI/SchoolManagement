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
using SchoolManagement.Domain.Common.Utils;

namespace SchoolManagement.Application.Academic.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _repository;
    private readonly ITeacherQueryService _queryService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;

    public TeacherService(
        ITeacherRepository repository,
        ITeacherQueryService queryService,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _queryService = queryService;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
    }

    public async Task<TeacherResponseDto> CreateAsync(TeacherCommand command)
    {
        // Generate unique slug from FirstName + LastName + Phone
        var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        var teacher = TeacherMapper.ToDomain(command);

        // Use repository for tracking operations
        await _repository.AddAsync(teacher);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Teacher",
            entityId: teacher.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(teacher));

        return TeacherMapper.ToResponse(teacher);
    }

    public async Task<TeacherResponseDto> UpdateAsync(Guid id, UpdateTeacherCommand command)
    {
        var teacher = await _repository.GetByIdAsync(id);
        if (teacher == null)
        {
            throw new NotFoundException($"Teacher with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(teacher);

        // Check if name or phone changed - regenerate slug if needed
        bool nameOrPhoneChanged = teacher.FirstName != command.FirstName || 
                                   teacher.LastName != command.LastName || 
                                   teacher.Phone != command.Phone;

        if (nameOrPhoneChanged)
        {
            var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
            teacher.UpdateSlug(command.Slug);
        }

        // Replace nonexistent UpdatePersonalInfo with existing methods
        teacher.UpdateFirstName(command.FirstName);
        teacher.UpdateLastName(command.LastName);
        teacher.UpdateEmail(command.Email);
        teacher.UpdatePhone(command.Phone);

        teacher.UpdateSalary(command.Salary);
        teacher.UpdateSpecialization(command.Specialization);

        await _repository.UpdateAsync(teacher);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Teacher",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(teacher));

        return TeacherMapper.ToResponse(teacher);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Use repository for tracking operations
        var teacher = await _repository.GetByIdAsync(id);
        if (teacher == null)
        {
            throw new NotFoundException($"Teacher with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(teacher);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Teacher",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    public async Task<TeacherResponseDto> GetByIdAsync(Guid id)
    {
        // Use query service for non-tracking read operations
        var teacher = await _queryService.GetResponseByIdAsync(id);
        if (teacher == null)
        {
            throw new NotFoundException($"Teacher with ID {id} not found.");
        }

        return teacher;
    }

    public async Task<List<TeacherResponseDto>> GetAllAsync()
    {
        // Use query service for non-tracking read operations
        return await _queryService.GetAllResponsesAsync();
    }

    private static object CreateAuditSnapshot(Teacher teacher)
    {
        return new
        {
            teacher.Id,
            teacher.FirstName,
            teacher.LastName,
            teacher.Slug,
            teacher.GenderId,
            teacher.Email,
            teacher.Phone,
            teacher.DateOfBirth,
            teacher.HireDate,
            teacher.Salary,
            teacher.BranchId,
            teacher.Specialization
        };
    }
}
