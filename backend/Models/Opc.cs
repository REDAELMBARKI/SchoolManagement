using SchoolManagement.Backend.Models;

public class Opc : Employee
{
    public virtual IEnumerable<Intake> Intakes {get;set;} = [] ;
}