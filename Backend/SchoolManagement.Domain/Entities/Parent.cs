using SchoolManagement.Domain.Exceptions;
using System.Collections.ObjectModel;

namespace SchoolManagement.Domain.Entities;

public class Parent : Person
{
    public string? Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public RelationshipType Relationship { get; private set; }
    public virtual ICollection<Student> Students { get; private set; } = new List<Student>();

    private Parent() { } // For EF Core

    public static Parent Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, RelationshipType relationship)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }

        var parent = new Parent
        {
            Email = email,
            Phone = phone,
            Relationship = relationship
        };

        parent.RegisterPerson(firstName, lastName, slug, genderId);
        return parent;
    }

    public void UpdateEmail(string? email)
    {
        Email = email;
    }

    public void UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }
        Phone = phone;
    }

    public void UpdateRelationship(RelationshipType relationship)
    {
        Relationship = relationship;
    }
}

public enum RelationshipType
{
    Father,
    Mother,
    Guardian,
    Grandfather,
    Grandmother,
    Uncle,
    Aunt,
    Other
}
