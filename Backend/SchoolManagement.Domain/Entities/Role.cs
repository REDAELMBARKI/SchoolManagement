using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Role : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public virtual List<User> Users { get; private set; } = new List<User>();

    private Role() { }

    public static Role Create(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Role name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Role slug cannot be empty.");
        }

        return new Role
        {
            Name = name,
            Slug = slug
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Role name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Role slug cannot be empty.");
        }
        Slug = slug;
    }
}
