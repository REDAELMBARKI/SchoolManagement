using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.ValueObjects;

namespace SchoolManagement.Domain.Entities;

public abstract class Employee : Person
{
    public Email? Email { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    public DateTime HireDate { get; private set; } = DateTime.UtcNow;
    public decimal Salary { get; private set; }

    // Foreign keys
    public Guid BranchId { get; private set; }

    // Navigation properties
    public virtual Branch Branch { get; private set; } = null!;

    protected void RegisterEmployee(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, DateOnly? dateOfBirth, DateTime hireDate, decimal salary, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone cannot be empty.");
        }
        if (salary < 0)
        {
            throw new DomainException("Salary cannot be negative.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        RegisterPerson(firstName, lastName, slug, genderId);
        Email = email != null ? new Email(email) : null;
        Phone = phone;
        DateOfBirth = dateOfBirth;
        HireDate = hireDate;
        Salary = salary;
        BranchId = branchId;
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

    public void UpdateDateOfBirth(DateOnly? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void UpdateHireDate(DateTime hireDate)
    {
        HireDate = hireDate;
    }

    public void UpdateSalary(decimal salary)
    {
        if (salary < 0)
        {
            throw new DomainException("Salary cannot be negative.");
        }
        Salary = salary;
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
