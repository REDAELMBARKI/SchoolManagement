using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.ValueObjects;

namespace SchoolManagement.Domain.Entities;

public class Student : Person
{
    public Email? Email { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }

    // fks
    public Guid? IntakeId { get; private set; }

    // navigation
    public virtual ICollection<Parent> Parents { get; private set; } = new List<Parent>();
    public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
    public virtual Intake? Intake { get; private set; }

    private Student() { } // For EF Core

    public static Student Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, DateOnly dateOfBirth, Guid? intakeId)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }

        var student = new Student
        {
            Email = email != null ? new Email(email) : null,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            IntakeId = intakeId
        };

        student.RegisterPerson(firstName, lastName, slug, genderId);
        return student;
    }

    public void UpdateEmail(string? email)
    {
        Email = email != null ? new Email(email) : null;
    }

    public void UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }
        Phone = phone;
    }

    public void UpdateDateOfBirth(DateOnly dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void UpdateIntakeId(Guid? intakeId)
    {
        IntakeId = intakeId;
    }
}
