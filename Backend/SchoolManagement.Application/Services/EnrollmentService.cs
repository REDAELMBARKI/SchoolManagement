using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Services;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;

    public EnrollmentService(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EnrollmentResponseDto>> GetAllAsync()
    {
        var enrollments = await _repository.GetAllAsync();
        return enrollments.Select(MapToDto).ToList();
    }

    public async Task<EnrollmentResponseDto?> GetByIdAsync(Guid id)
    {
        var enrollment = await _repository.GetByIdAsync(id);
        if (enrollment is null) throw new NotFoundException($"Enrollment with id {id} not found");
        return MapToDto(enrollment);
    }

    public async Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto dto)
    {
        // TODO: Business logic for you to implement:
        // - Check if student exists
        // - Check if group has capacity
        // - Check if student already enrolled in this subject
        // - Calculate initial fees based on Plan

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            SubjectId = dto.SubjectId,
            GroupId = dto.GroupId,
            BranchId = dto.BranchId,
            PlanId = dto.PlanId,
            Notes = dto.Notes,
            Status = "Active",
            EnrolledAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(enrollment);
        return MapToDto(created);
    }

    public async Task<EnrollmentResponseDto> UpdateAsync(Guid id, EnrollmentRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Enrollment with id {id} not found");

        // TODO: Business logic for you to implement:
        // - Validate status transitions (Active -> Dropped, etc.)
        // - Handle group change (check new group capacity)

        var enrollment = new Enrollment
        {
            Id = id,
            StudentId = dto.StudentId,
            SubjectId = dto.SubjectId,
            GroupId = dto.GroupId,
            BranchId = dto.BranchId,
            PlanId = dto.PlanId,
            Notes = dto.Notes,
            Status = existing.Status,
            EnrolledAt = existing.EnrolledAt
        };

        await _repository.UpdateAsync(id, enrollment);
        return await GetByIdAsync(id) ?? throw new NotFoundException($"Enrollment with id {id} not found after update");
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    // TODO: Additional methods for you to implement:
    // - EnrollStudentAsync (with all validations)
    // - DropEnrollmentAsync (change status to Dropped)
    // - GetStudentEnrollmentsAsync (by student id)
    // - GetGroupEnrollmentsAsync (by group id)
    // - ProcessPaymentAsync (create payment record)


}
