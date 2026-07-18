using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Subject : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid BranchId { get; private set; }

    public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
    public virtual ICollection<Group> Groups { get; private set; } = new List<Group>();
    public virtual ICollection<Teacher> Teachers { get; private set; } = new List<Teacher>();
    public virtual Branch Branch { get; private set; } = null!;

    private Subject() { }

    public static Subject Create(string name, string slug, string? description, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Subject name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Subject slug cannot be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        return new Subject
        {
            Name = name,
            Slug = slug,
            Description = description,
            BranchId = branchId
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Subject name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Subject slug cannot be empty.");
        }
        Slug = slug;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }
}
