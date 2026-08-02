using SchoolManagement.Domain.Academic.Entities;

using SchoolManagement.Domain.Common;

using SchoolManagement.Domain.Common.Entities;

using SchoolManagement.Domain.Common.Exceptions;

using SchoolManagement.Domain.Core.DomainEvents;

using SchoolManagement.Domain.Core.Enums;



namespace SchoolManagement.Domain.Core.Entities;



public class Enrollment : AggregateRoot

{

    public DateTime EnrolledAt { get; private set; } = DateTime.UtcNow;

    public DateTime? DroppedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;

    public string? Notes { get; private set; }

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

        string? notes = null)

    {

        if (studentId == Guid.Empty)

            throw new DomainException("Student ID must not be empty.");

        if (subjectId == Guid.Empty)

            throw new DomainException("Subject ID must not be empty.");

        if (groupId == Guid.Empty)

            throw new DomainException("Group ID must not be empty.");

        if (branchId == Guid.Empty)

            throw new DomainException("Branch ID must not be empty.");



        var enrollment = new Enrollment

        {

            StudentId = studentId,

            SubjectId = subjectId,

            GroupId = groupId,

            BranchId = branchId,

            EnrolledAt = enrolledAt ?? DateTime.UtcNow,

            Status = status,

            Notes = notes

        };



        if (planId.HasValue && planId.Value != Guid.Empty)

        {

            enrollment.AddPlan(planId.Value);

        }

        enrollment.AddDomainEvent(new EnrollmentCreatedDomainEvent(

            enrollment.Id,

            enrollment.StudentId,

            enrollment.BranchId,

            enrollment.EnrolledAt));

        return enrollment;

    }



    public void AddPlan(Guid planId)

    {

        EnsureFeesNotLocked();



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



    public void CompleteEnrollment(string? notes = null)

    {

        if (Status == EnrollmentStatus.Completed)

            throw new DomainException("Enrollment is already completed.");

        if (Status != EnrollmentStatus.Active)

            throw new DomainException("Only active enrollments can be completed.");



        Status = EnrollmentStatus.Completed;

        CompletedAt = DateTime.UtcNow;



        if (notes != null)

            Notes = notes;



        AddDomainEvent(new EnrollmentCompletedDomainEvent(Id, GroupId, CompletedAt.Value, notes));

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



    public void TransferGroup(Guid newGroupId, string reason)
    {
        if (newGroupId == Guid.Empty)
            throw new DomainException("New group ID must not be empty.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Transfer reason is required.");
        if (Status != EnrollmentStatus.Active)
            throw new DomainException("Only active enrollments can be transferred.");
        if (GroupId == newGroupId)
            throw new DomainException("Student is already in this group.");

        var oldGroupId = GroupId;
        GroupId = newGroupId;
        Notes = $"Transferred from group {oldGroupId} to {newGroupId}. Reason: {reason}";
        
        AddDomainEvent(new EnrollmentGroupTransferredDomainEvent(Id, StudentId, oldGroupId, newGroupId, reason, DateTime.UtcNow));
    }



    private void EnsureFeesNotLocked()

    {

        if (Status == EnrollmentStatus.Completed)

            throw new DomainException("Fee modifications are locked for this enrollment.");

    }

}
