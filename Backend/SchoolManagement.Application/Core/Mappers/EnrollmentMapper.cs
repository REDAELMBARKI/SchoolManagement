using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class EnrollmentMapper  
{
    public static Enrollment ToDomain(EnrollmentCommand command)
    {
        return Enrollment.Create(
            branchId : command.BranchId, 
            studentId: command.StudentId,
            subjectId: command.SubjectId,
            groupId: command.PreferedGroupId!.Value,
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
            DroppedAt = e.DroppedAt,
            CompletedAt = e.CompletedAt,
            Status = e.Status,
            Notes = e.Notes,
            StudentId = e.StudentId,
            SubjectId = e.SubjectId,
            GroupId = e.GroupId,
            BranchId = e.BranchId,
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
            Payments = e.Payments?.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                EnrollmentId = p.EnrollmentId,
                InvoiceId = p.InvoiceId,
                Amount = p.Amount,
                TransferFees = p.TransferFees,
                Method = p.Method,
                Status = p.Status,
                PaidAt = p.PaidAt,
                BranchId = p.BranchId,
                ReceivedByStaffId = p.ReceivedByStaffId,
                ExternalReferenceCode = p.ExternalReferenceCode,
                MethodDetailsJson = p.MethodDetailsJson
            }).ToList() ?? new List<PaymentResponseDto>(),
            EnrollmentPlans = e.EnrollmentPlans?.Select(ep => new EnrollmentPlanResponseDto
            {
                Id = ep.Id,
                EnrollmentId = ep.EnrollmentId,
                PlanId = ep.PlanId,
                CreatedAt = ep.CreatedAt,
                Plan = ep.Plan != null ? new PlanResponseDto
                {
                    Id = ep.Plan.Id,
                    Name = ep.Plan.Name,
                    DurationMonths = ep.Plan.DurationMonths,
                    BaseAmount = ep.Plan.BaseAmount,
                    DiscountPercent = ep.Plan.DiscountPercent,
                    IsActive = ep.Plan.IsActive,
                    RemainingAmountDueDays = ep.Plan.RemainingAmountDueDays,
                    BranchId = ep.Plan.BranchId
                } : null
            }).ToList() ?? new List<EnrollmentPlanResponseDto>()
        };
    }
}
