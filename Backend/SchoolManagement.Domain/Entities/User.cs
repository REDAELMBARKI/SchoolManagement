
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.ValueObjects;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolManagement.Domain.Entities;

public class User : Person
{

    public Email Email { get; private set; } = null!;
    public DateOnly? DateOfBirth { get; private set; }
    public string Password { get; private set; } = string.Empty;
    public bool IsActivated { get; private set; }

    // navigation
    public List<Role> Roles { get; private set; } = new List<Role>();

    private User() { }

    public static User Register(string firstName, string lastName, string slug, Guid? genderId, string email, string password, DateOnly? dateOfBirth, bool isActivated = false)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password cannot be empty.");

        var user = new User
        {
            Email = new Email(email),
            Password = password,
            DateOfBirth = dateOfBirth,
            IsActivated = isActivated
        };
        user.RegisterPerson(firstName, lastName, slug, genderId);
        return user;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");
        Email = new Email(email);
    }

    public void UpdatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password cannot be empty.");
        Password = password;
    }

    public void UpdateDateOfBirth(DateOnly? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void UpdateIsActivated(bool isActivated)
    {
        IsActivated = isActivated;
    }
}


