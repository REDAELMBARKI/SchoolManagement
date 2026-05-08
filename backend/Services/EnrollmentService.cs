using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Repositories;

namespace SchoolManagement.Backend.Services;

public class EnrollmentService
{
    private readonly EnrollmentRepository _repository;

    public EnrollmentService(EnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EnrollmentResponseDto>> GetAllAsync()
    {
        var enrollments = await _repository.GetAllAsync();
        return enrollments.Select(MapToDto).ToList();
    }

    public async Task<EnrollmentResponseDto?> GetByIdAsync(int id)
    {
        var enrollment = await _repository.GetOneAsync(id);
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

    public async Task<EnrollmentResponseDto> UpdateAsync(int id, EnrollmentRequestDto dto)
    {
        var existing = await _repository.GetOneAsync(id);
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

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    // TODO: Additional methods for you to implement:
    // - EnrollStudentAsync (with all validations)
    // - DropEnrollmentAsync (change status to Dropped)
    // - GetStudentEnrollmentsAsync (by student id)
    // - GetGroupEnrollmentsAsync (by group id)
    // - ProcessPaymentAsync (create payment record)

    private static EnrollmentResponseDto MapToDto(Enrollment e)
    {
        return new EnrollmentResponseDto
        {
            Id = e.Id,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status,
            StudentId = e.StudentId,
            SubjectId = e.SubjectId,
            GroupId = e.GroupId,
            BranchId = e.BranchId,
            PlanId = e.PlanId,
            Student = e.Student != null ? new StudentResponseDto
            {
                Id = e.Student.Id,
                FirstName = e.Student.FirstName,
                LastName = e.Student.LastName,
                Slug = e.Student.Slug,
                Email = e.Student.Email,
                Phone = e.Student.Phone,
                DateOfBirth = e.Student.DateOfBirth
            } : null,
            Subject = e.Subject != null ? new SubjectResponseDto
            {
                Id = e.Subject.Id,
                Name = e.Subject.Name,
                Slug = e.Subject.Slug,
                Description = e.Subject.Description
            } : null,
            Group = e.Group != null ? new GroupResponseDto
            {
                Id = e.Group.Id,
                Name = e.Group.Name,
                Capacity = e.Group.Capacity,
                Period = e.Group.Period
            } : null,
            Branch = e.Branch != null ? new BranchResponseDto
            {
                Id = e.Branch.Id,
                Slug = e.Branch.Slug,
                Name = e.Branch.Name,
                City = e.Branch.City,
                Address = e.Branch.Address,
                Phone = e.Branch.Phone
            } : null,
            Plan = e.Plan != null ? new PlanResponseDto
            {
                Id = e.Plan.Id,
                Name = e.Plan.Name,
                DurationMonths = e.Plan.DurationMonths,
                DiscountPercent = e.Plan.DiscountPercent
            } : null,
            Payments = e.Payments?.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                AmountPaid = p.AmountPaid,
                FeeAmount = p.FeeAmount,
                Status = p.Status,
                PaidAt = p.PaidAt,
                PeriodStart = p.PeriodStart,
                PeriodEnd = p.PeriodEnd
            }).ToList() ?? new List<PaymentResponseDto>()
        };
    }
}
