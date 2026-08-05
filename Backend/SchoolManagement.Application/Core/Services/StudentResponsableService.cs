using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.Services;

public class StudentResponsableService : IStudentResponsableService
{
    private readonly IStudentResponsableRepository _responsableRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;

    public StudentResponsableService(
        IStudentResponsableRepository responsableRepository,
        IStudentRepository studentRepository,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _responsableRepository = responsableRepository;
        _studentRepository = studentRepository;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<StudentResponsableResponseDto> CreateAndLinkToStudentAsync(
        Guid studentId, 
        StudentResponsableRequestDto request)
    {
        // 1. Generate slug for parent/guardian
        var responsableSlug = await CustomSluger.Slug(
            slug => _responsableRepository.IsExistsBySlugAsync(slug),
            $"{request.FirstName}-{request.LastName}"
        );

        // 2. Create StudentResponsable entity
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

        // 3. Link parent to student via many-to-many relationship
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student != null)
        {
            student.StudentResponsables.Add(createdResponsable);
            await _studentRepository.UpdateAsync(student);
        }

        // 4. Audit log
        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
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
}
