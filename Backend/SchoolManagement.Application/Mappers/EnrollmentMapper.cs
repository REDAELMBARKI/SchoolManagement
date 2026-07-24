using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System.Linq;

namespace SchoolManagement.Application.Mappers;

public static class EnrollmentMapper  
{
    public static Enrollment ToDomain(EnrollmentCommand command)
    {
        return Enrollment.Create(
            branchId : command.BranchId, 
            studentId: command.StudentId,
            subjectId: command.SubjectId,
            groupId: command.GroupId,
            planId: command.PlanId,
            enrolledAt: command.EnrolledAt,
            status: command.Status,
            notes: command.Notes
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
                BaseAmount = e.Plan.BaseAmount,
                DiscountPercent = e.Plan.DiscountPercent,
                IsActive = e.Plan.IsActive,
                RemainingAmountDueDate = e.Plan.RemainingAmountDueDate
            } : null,
            Payments = e.Payments?.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                EnrollmentId = p.EnrollmentId,
                Amount = p.Amount,
                TransferFees = p.TransferFees,
                Method = p.Method,
                Status = p.Status,
                PaidAt = p.PaidAt,
                BranchId = p.BranchId,
                ReceivedByStaffId = p.ReceivedByStaffId,
                ExternalReferenceCode = p.ExternalReferenceCode,
                MethodDetailsJson = p.MethodDetailsJson
            }).ToList() ?? new List<PaymentResponseDto>()
        };
    }
}
