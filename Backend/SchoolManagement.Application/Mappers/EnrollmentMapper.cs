using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System.Linq;

namespace SchoolManagement.Application.Mappers;

public static class EnrollmentMapper  
{
    public static EnrollmentResponseDto ToResponse(Enrollment enrollment)
    {
        return new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status,
            Notes = enrollment.Notes,
            StudentId = enrollment.StudentId,
            SubjectId = enrollment.SubjectId,
            GroupId = enrollment.GroupId,
            BranchId = enrollment.BranchId,
            PlanId = enrollment.PlanId,
            Payments = enrollment.Payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                AmountPaid = p.AmountPaid,
                FeeAmount = p.FeeAmount,
                Status = p.Status,
                PaidAt = p.PaidAt,
                PeriodStart = p.PeriodStart,
                PeriodEnd = p.PeriodEnd,
                EnrollmentId = p.EnrollmentId
            }).ToList()
        };
    }
}
