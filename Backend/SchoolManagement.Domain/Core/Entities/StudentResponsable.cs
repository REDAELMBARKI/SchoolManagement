using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Domain.Core.Entities;

public class StudentResponsable : Person
{
    public string? Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public RelationshipType Relationship { get; private set; }
    // BranchId is inherited from Person base class

    public virtual ICollection<Student> Students { get; private set; } = new List<Student>();
    // Branch is inherited from Person base class

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
            Relationship = relationship
        };

        responsable.RegisterPerson(firstName, lastName, slug, genderId, branchId);
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

  
}

