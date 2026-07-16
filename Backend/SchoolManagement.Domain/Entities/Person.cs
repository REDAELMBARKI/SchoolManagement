using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Utils;

namespace SchoolManagement.Domain.Entities;

public abstract class Person : AggregateRoot
{
    public string FirstName { get; protected set; } = string.Empty;
    public string LastName { get; protected set; } = string.Empty;
    public string Slug { get; protected set; } = string.Empty;
    public Guid? GenderId { get; protected set; }
    public virtual Gender? Gender { get; protected set; }

    protected void RegisterPerson(string firstName, string lastName, Guid? genderId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("Last name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Slug cannot be empty.");
        }

        FirstName = firstName;
        LastName = lastName;
        GenderId = genderId;
    }

    public void UpdateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("First name cannot be empty.");
        }
        FirstName = firstName;
    }

    public void UpdateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("Last name cannot be empty.");
        }
        LastName = lastName;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Slug cannot be empty.");
        }
        Slug = slug;
    }

    public void UpdateGenderId(Guid? genderId)
    {
        GenderId = genderId;
    }
}
