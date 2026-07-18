using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Teacher : Employee
{
    public string Specialization { get; private set; } = string.Empty;

    // navigation
    public virtual ICollection<Group> Groups { get; private set; } = new List<Group>();
    public virtual ICollection<TeacherSubject> TeacherSubjects { get; private set; } = new List<TeacherSubject>();

    private Teacher() { } // For EF Core

    public static Teacher Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, DateOnly? dateOfBirth, DateTime hireDate, decimal salary, Guid branchId, string specialization)
    {
        if (string.IsNullOrWhiteSpace(specialization))
        {
            throw new DomainException("Specialization cannot be empty.");
        }

        var teacher = new Teacher
        {
            Specialization = specialization
        };

        teacher.RegisterEmployee(firstName, lastName, slug, genderId, email, phone, dateOfBirth, hireDate, salary, branchId);
        return teacher;
    }

    public void UpdateSpecialization(string specialization)
    {
        if (string.IsNullOrWhiteSpace(specialization))
        {
            throw new DomainException("Specialization cannot be empty.");
        }
        Specialization = specialization;
    }
}
