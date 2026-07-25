using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class User : Person
{
    public string UserName { get; private set; } = string.Empty;
    public string NormalizedUserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public virtual ICollection<UserRole> Roles { get; private set; } = new List<UserRole>();

    private User() { }

    public static User Register(string userName, string firstName, string lastName, string slug, Guid? genderId, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("Username cannot be empty.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty.");

        var user = new User
        {
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            PasswordHash = passwordHash
        };
        user.RegisterPerson(firstName, lastName, slug, genderId);
        return user;
    }

    public void UpdateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("Username cannot be empty.");
        UserName = userName;
        NormalizedUserName = userName.ToUpperInvariant();
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty.");
        PasswordHash = passwordHash;
    }
}

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private UserRole() { }

    public static UserRole Create(Guid userId, Guid roleId, string name)
    {
        if (userId == Guid.Empty) throw new DomainException("User ID must not be empty.");
        if (roleId == Guid.Empty) throw new DomainException("Role ID must not be empty.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Role name cannot be empty.");
        return new UserRole { UserId = userId, RoleId = roleId, Name = name };
    }
}
