using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.ValueObjects;

namespace SchoolManagement.Domain.Core.Entities;

public class Student : Person
{
    public Email? Email { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public bool IsDirectRegistration { get; private set; }
    public decimal CreditBalance { get; private set; }

    // fks
    public Guid? IntakeId { get; private set; }
    // BranchId is inherited from Person base class

    // navigation
    public virtual ICollection<StudentResponsable> StudentResponsables { get; private set; } = new List<StudentResponsable>();
    public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
    public virtual Intake? Intake { get; private set; }
    // Branch is inherited from Person base class

    private Student() { }

    public static Student Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, DateOnly dateOfBirth, Guid? intakeId, bool isDirectRegistration, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }

        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        // Validate: either IntakeId is provided OR IsDirectRegistration is true
        if (!intakeId.HasValue && !isDirectRegistration)
        {
            throw new DomainException("Either IntakeId must be provided or IsDirectRegistration must be true.");
        }

        // If IntakeId is provided, IsDirectRegistration should be false (optional, but good to enforce)
        if (intakeId.HasValue && isDirectRegistration)
        {
            throw new DomainException("Cannot set both IntakeId and IsDirectRegistration to true.");
        }

        var student = new Student
        {
            Email = string.IsNullOrWhiteSpace(email) ? null : new Email(email),
            Phone = phone,
            DateOfBirth = dateOfBirth,
            IntakeId = intakeId,
            IsDirectRegistration = isDirectRegistration
        };

        student.RegisterPerson(firstName, lastName, slug, genderId, branchId);
        return student;
    }

    public void UpdateEmail(string? email)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : new Email(email);
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
        // Validate: if we're setting IntakeId, IsDirectRegistration must be false
        if (intakeId.HasValue && IsDirectRegistration)
        {
            throw new DomainException("Cannot set IntakeId when IsDirectRegistration is true.");
        }

        // Validate: if we're unsetting IntakeId, IsDirectRegistration must be true
        if (!intakeId.HasValue && !IsDirectRegistration)
        {
            throw new DomainException("Either IntakeId must be provided or IsDirectRegistration must be true.");
        }

        IntakeId = intakeId;
    }

    public void UpdateRegistrationSource(Guid? intakeId, bool isDirectRegistration)
    {
        if (!intakeId.HasValue && !isDirectRegistration)
        {
            throw new DomainException("Either IntakeId must be provided or IsDirectRegistration must be true.");
        }

        if (intakeId.HasValue && isDirectRegistration)
        {
            throw new DomainException("Cannot set IntakeId when IsDirectRegistration is true.");
        }

        IntakeId = intakeId;
        IsDirectRegistration = isDirectRegistration;
    }

    public void UpdateIsDirectRegistration(bool isDirectRegistration)
    {
        // Validate: if we're setting IsDirectRegistration to true, IntakeId must be null
        if (isDirectRegistration && IntakeId.HasValue)
        {
            throw new DomainException("Cannot set IsDirectRegistration to true when IntakeId is provided.");
        }

        // Validate: if we're setting IsDirectRegistration to false, IntakeId must be provided
        if (!isDirectRegistration && !IntakeId.HasValue)
        {
            throw new DomainException("Either IntakeId must be provided or IsDirectRegistration must be true.");
        }

        IsDirectRegistration = isDirectRegistration;
    }

    // UpdateBranchId is inherited from Person base class

    public void AddCredit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be greater than zero.");
        CreditBalance += amount;
    }

    public void UseCredit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Credit amount must be greater than zero.");
        if (amount > CreditBalance)
            throw new DomainException($"Insufficient credit balance. Available: {CreditBalance:C}, Required: {amount:C}");
        CreditBalance -= amount;
    }

    public void UpdateCreditBalance(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Credit balance cannot be negative.");
        CreditBalance = amount;
    }
}
