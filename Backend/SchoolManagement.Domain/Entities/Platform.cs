using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Platform : AggregateRoot
{
    public string Slug { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }

    public virtual Branch Branch { get; private set; } = null!;

    private Platform() { }

    public static Platform Create(string name, string slug, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Platform name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Platform slug cannot be empty.");
        }
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");

        return new Platform
        {
            Name = name,
            Slug = slug,
            BranchId = branchId
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Platform name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Platform slug cannot be empty.");
        }
        Slug = slug;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        BranchId = branchId;
    }
}
