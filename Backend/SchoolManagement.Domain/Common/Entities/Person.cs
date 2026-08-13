using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Domain.Common.Entities;

public abstract class Person : AggregateRoot
{
    public string FirstName { get; protected set; } = string.Empty;
    public string LastName { get; protected set; } = string.Empty;
    public string Slug { get; protected set; } = string.Empty;
    public Guid? GenderId { get; protected set; }
    public virtual Gender? Gender { get; protected set; }
    
    // Branch isolation - required (use Branch.SYSTEM_BRANCH_ID for SuperAdmin)
    public Guid BranchId { get; protected set; }
    public virtual Branch? Branch { get; protected set; }

    protected void RegisterPerson(string firstName, string lastName, string slug, Guid? genderId, Guid branchId)
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
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        FirstName = firstName;
        LastName = lastName;
        Slug = slug;
        GenderId = genderId;
        BranchId = branchId;
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
    
    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }
}
