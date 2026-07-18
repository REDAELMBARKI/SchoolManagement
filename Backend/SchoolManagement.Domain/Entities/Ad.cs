using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Ad : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public Guid PlatformId { get; private set; }
    public Guid BranchId { get; private set; }
    public virtual Branch Branch { get; private set; } = null!;
    public virtual Platform Platform { get; private set; } = null!;

    private Ad() { }

    public static Ad Create(string name, string slug, Guid platformId, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Ad name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Ad slug cannot be empty.");
        }
        if (platformId == Guid.Empty)
        {
            throw new DomainException("Platform ID must not be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        return new Ad
        {
            Name = name,
            Slug = slug,
            PlatformId = platformId,
            BranchId = branchId
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Ad name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Ad slug cannot be empty.");
        }
        Slug = slug;
    }

    public void UpdatePlatformId(Guid platformId)
    {
        if (platformId == Guid.Empty)
        {
            throw new DomainException("Platform ID must not be empty.");
        }
        PlatformId = platformId;
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
