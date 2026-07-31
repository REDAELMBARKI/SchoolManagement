using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.DomainEvents.Enrollments;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities.EnrollmentAggregate;

public class Enrollment : AggregateRoot
{
    public DateTime EnrolledAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DroppedAt { get; private set; }
    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;  // Active / Dropped / Completed
    public string? Notes { get; private set; }
    public decimal CreditBalance { get; private set; }

    // FKs
    public Guid StudentId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid BranchId { get; private set; }

    // navigations
    public virtual IEnumerable<Payment> Payments { get; private set; } = new List<Payment>();
    public virtual ICollection<EnrollmentPlan> EnrollmentPlans { get; private set; } = new List<EnrollmentPlan>();
    public virtual Subject Subject { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;
    public virtual Group Group { get; private set; } = null!;

    private Enrollment() { }

    public static Enrollment Create(
        Guid studentId,
        Guid subjectId,
        Guid groupId,
        Guid branchId,
        Guid? planId = null,
        DateTime? enrolledAt = null,
        EnrollmentStatus status = EnrollmentStatus.Active,
        string? notes = null,
        decimal creditBalance = 0)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Student ID must not be empty.");
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (creditBalance < 0)
            throw new DomainException("Credit balance cannot be negative.");

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            SubjectId = subjectId,
            GroupId = groupId,
            BranchId = branchId,
            EnrolledAt = enrolledAt ?? DateTime.UtcNow,
            Status = status,
            Notes = notes,
            CreditBalance = creditBalance
        };

        if (planId.HasValue && planId.Value != Guid.Empty)
        {
            enrollment.AddPlan(planId.Value);
        }

        return enrollment;
    }

    public void AddPlan(Guid planId)
    {
        if (planId == Guid.Empty)
            throw new DomainException("Plan ID must not be empty.");

        EnrollmentPlans.Add(EnrollmentPlan.Create(Id, planId));
    }

    public Plan? GetLatestPlan()
    {
        return EnrollmentPlans
            .OrderByDescending(ep => ep.CreatedAt)
            .Select(ep => ep.Plan)
            .FirstOrDefault();
    }

    public void UpdateEnrolledAt(DateTime enrolledAt)
    {
        EnrolledAt = enrolledAt;
    }

    public void DropEnrollment(string reason, DateTime? droppedAt = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Drop reason is required.");
        if (Status == EnrollmentStatus.Dropped)
            throw new DomainException("Enrollment is already dropped.");
        if (Status != EnrollmentStatus.Active)
            throw new DomainException("Only active enrollments can be dropped.");

        Status = EnrollmentStatus.Dropped;
        DroppedAt = droppedAt ?? DateTime.UtcNow;

        AddDomainEvent(new EnrollmentDroppedDomainEvent(Id, GroupId, reason, DroppedAt.Value));
    }

    public void UpdateStatus(EnrollmentStatus status)
    {
        if (string.IsNullOrWhiteSpace(status.ToString()))
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

    public void AddCredit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be greater than zero.");
        CreditBalance += amount;
    }

    public void UseCredit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be greater than zero.");
        if (amount > CreditBalance)
            throw new DomainException("Insufficient credit balance.");
        CreditBalance -= amount;
    }

    public void UpdateCreditBalance(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Credit balance cannot be negative.");
        CreditBalance = amount;
    }
}


