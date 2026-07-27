using SchoolManagement.Domain.Exceptions;
using System.Collections.ObjectModel;

namespace SchoolManagement.Domain.Entities;

public class StudentResponsable : Person
{
    public string? Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public RelationshipType Relationship { get; private set; }
    public Guid BranchId { get; private set; }

    public virtual ICollection<Student> Students { get; private set; } = new List<Student>();
    public virtual Branch Branch { get; private set; } = null!;

    private StudentResponsable() { }

    public static StudentResponsable Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, RelationshipType relationship, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        var responsable = new StudentResponsable
        {
            Email = email,
            Phone = phone,
            Relationship = relationship,
            BranchId = branchId
        };

        responsable.RegisterPerson(firstName, lastName, slug, genderId);
        return responsable;
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

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        BranchId = branchId;
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
