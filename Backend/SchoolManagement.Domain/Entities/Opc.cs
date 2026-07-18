using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Domain.Entities;

public class Opc : Employee
{
    public virtual ICollection<LeadSource> LeadSources { get; private set; } = new List<LeadSource>();

    [NotMapped]
    public IEnumerable<Intake> Intakes => LeadSources.SelectMany(ls => ls.Intakes).ToList();

    private Opc() { } // For EF Core

    public static Opc Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, DateOnly? dateOfBirth, DateTime hireDate, decimal salary, Guid branchId)
    {
        var opc = new Opc();
        opc.RegisterEmployee(firstName, lastName, slug, genderId, email, phone, dateOfBirth, hireDate, salary, branchId);
        return opc;
    }
}
