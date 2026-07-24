using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.Payment;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities.EnrollmentAggregate;

public class Enrollment : AggregateRoot
{
    public DateTime EnrolledAt { get; private set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active ;  // Active / Dropped / Completed
    public string? Notes { get; private set; }
    // FKs
    public Guid StudentId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid PlanId { get; private set; }
    // navigations
    public virtual IEnumerable<PaymentAllocation> Allocations { get; private set; } =  new List<PaymentAllocation>();

    public virtual Subject Subject { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;
    public virtual Plan Plan { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;
    public virtual Group Group { get; private set; } = null!;

    private Enrollment() { }

    public static Enrollment Create(Guid studentId, Guid subjectId, Guid groupId, Guid branchId, Guid planId, DateTime? enrolledAt = null, string status = "Active", string? notes = null)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Student ID must not be empty.");
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (planId == Guid.Empty)
            throw new DomainException("Plan ID must not be empty.");

        return new Enrollment
        {
            StudentId = studentId,
            SubjectId = subjectId,
            GroupId = groupId,
            BranchId = branchId,
            PlanId = planId,
            EnrolledAt = enrolledAt ?? DateTime.UtcNow,
            Status = status,
            Notes = notes
        };
    }

    public void UpdateEnrolledAt(DateTime enrolledAt)
    {
        EnrolledAt = enrolledAt;
    }

    public void UpdateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty.");
        Status = status;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    public void UpdateStudentId(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Student ID must not be empty.");
        StudentId = studentId;
    }

    public void UpdateSubjectId(Guid subjectId)
    {
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");
        SubjectId = subjectId;
    }

    public void UpdateGroupId(Guid groupId)
    {
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");
        GroupId = groupId;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        BranchId = branchId;
    }

    public void UpdatePlanId(Guid planId)
    {
        if (planId == Guid.Empty)
            throw new DomainException("Plan ID must not be empty.");
        PlanId = planId;
    }
}
