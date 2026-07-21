namespace SchoolManagement.Domain.Entities;

public class CommercialAgent : Employee
{
    // navigation
    public virtual ICollection<Intake> Intakes { get; private set; } = new List<Intake>();

    private CommercialAgent() { } 

    public static CommercialAgent Register(string firstName, string lastName, string slug, Guid? genderId, string? email, string phone, DateOnly? dateOfBirth, DateTime hireDate, decimal salary, Guid branchId)
    {
        var agent = new CommercialAgent();
        agent.RegisterEmployee(firstName, lastName, slug, genderId, email, phone, dateOfBirth, hireDate, salary, branchId);
        return agent;
    }
}
