using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System.Linq;

namespace SchoolManagement.Application.Mappers;

public static class EnrollmentMapper  
{
    public static Enrollment ToDomain(EnrollmentRequestDto dto)
    {
        return Enrollment.Create(
            studentId: dto.StudentId,
            subjectId: dto.SubjectId,
            groupId: dto.GroupId,
            branchId: dto.BranchId,
            planId: dto.PlanId,
            notes: dto.Notes
        );
    }

    public static EnrollmentResponseDto ToResponse(Enrollment e)
    {
        return new EnrollmentResponseDto
        {
            Id = e.Id,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status,
            Notes = e.Notes,
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
                Email = e.Student.Email?.Value ?? string.Empty,
                Phone = e.Student.Phone,
                DateOfBirth = e.Student.DateOfBirth
            } : null,
            Subject = e.Subject != null ? new SubjectResponseDto
            {
                Id = e.Subject.Id,
                Name = e.Subject.Name,
                Slug = e.Subject.Slug,
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
