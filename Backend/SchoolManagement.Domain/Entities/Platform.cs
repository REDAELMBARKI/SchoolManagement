using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Platform : AggregateRoot
{
    public string Slug { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    private Platform() { }

    public static Platform Create(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Platform name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Platform slug cannot be empty.");
        }

        return new Platform
        {
            Name = name,
            Slug = slug
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
}
