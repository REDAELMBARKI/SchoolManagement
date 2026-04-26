using SchoolManagement.Backend.Models;

public class Opc : Employee
{
    protected IEnumerable<Intake> Intakes {get;set;} = [] ;
}