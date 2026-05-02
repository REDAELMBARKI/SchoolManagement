using SchoolManagement.Backend.Models;

public class Opc : Employee
{
    public IEnumerable<Intake> Intakes {get;set;} = [] ;
}