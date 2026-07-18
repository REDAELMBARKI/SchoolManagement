using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Group : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; } = 15;
    // Morning / Afternoon / Evening / Weekend
    public string Period { get; private set; } = string.Empty;

    // FKs
    public Guid BranchId { get; private set; }
    public Guid LevelId { get; private set; }
    public Guid SubjectId { get; private set; }

    // navigations
    public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
    public virtual ICollection<GroupTeacher> Teachers { get; private set; } = new List<GroupTeacher>();
    public virtual Branch Branch { get; private set; } = null!;
    public virtual Level Level { get; private set; } = null!;
    public virtual Subject Subject { get; private set; } = null!;

    private Group() { } // For EF Core

    public static Group Create(string name, int capacity, string period, Guid branchId, Guid levelId, Guid subjectId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Group name cannot be empty.");
        }
        if (capacity <= 0)
        {
            throw new DomainException("Capacity must be greater than zero.");
        }
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new DomainException("Period cannot be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        if (levelId == Guid.Empty)
        {
            throw new DomainException("Level ID must not be empty.");
        }
        if (subjectId == Guid.Empty)
        {
            throw new DomainException("Subject ID must not be empty.");
        }

        return new Group
        {
            Name = name,
            Capacity = capacity,
            Period = period,
            BranchId = branchId,
            LevelId = levelId,
            SubjectId = subjectId
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Group name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            throw new DomainException("Capacity must be greater than zero.");
        }
        Capacity = capacity;
    }

    public void UpdatePeriod(string period)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new DomainException("Period cannot be empty.");
        }
        Period = period;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }

    public void UpdateLevelId(Guid levelId)
    {
        if (levelId == Guid.Empty)
        {
            throw new DomainException("Level ID must not be empty.");
        }
        LevelId = levelId;
    }

    public void UpdateSubjectId(Guid subjectId)
    {
        if (subjectId == Guid.Empty)
        {
            throw new DomainException("Subject ID must not be empty.");
        }
        SubjectId = subjectId;
    }
}
