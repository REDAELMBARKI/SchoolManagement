using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities.EnrollmentAggregate;

public class EnrollmentPlan : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public Guid PlanId { get; private set; }

    public virtual Enrollment Enrollment { get; private set; } = null!;
    public virtual Plan Plan { get; private set; } = null!;

    private EnrollmentPlan() { }

    public static EnrollmentPlan Create(Guid enrollmentId, Guid planId)
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID must not be empty.");
        if (planId == Guid.Empty)
            throw new DomainException("Plan ID must not be empty.");

        return new EnrollmentPlan
        {
            EnrollmentId = enrollmentId,
            PlanId = planId,
        };
    }

}
