using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using System;

namespace SchoolManagement.Domain.Entities;

public class Gender : AggregateRoot
{
    public string Slug { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    private Gender() { }

    public static Gender Create(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Gender name cannot be empty.");
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Gender slug cannot be empty.");

        return new Gender
        {
            Name = name,
            Slug = slug
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Gender name cannot be empty.");
        Name = name;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Gender slug cannot be empty.");
        Slug = slug;
    }
}
