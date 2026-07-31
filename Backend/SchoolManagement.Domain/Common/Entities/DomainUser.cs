using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Domain.Common.Entities;

public class DomainUser : Person
{
    public string? ApplicationUserId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime? LastActiveAt { get; private set; }

    private DomainUser() { }

    public static DomainUser Register(string firstName, string lastName, string slug, Guid? genderId, string? applicationUserId = null)
    {
        var user = new DomainUser
        {
            ApplicationUserId = applicationUserId,
            IsActive = true
        };
        user.RegisterPerson(firstName, lastName, slug, genderId);
        return user;
    }

    public void LinkToApplicationUser(string applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new DomainException("Application user ID cannot be empty.");
        ApplicationUserId = applicationUserId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void MarkActiveNow()
    {
        LastActiveAt = DateTime.UtcNow;
    }
}
