using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Entities;

public class Opc : Employee
{ 
    public virtual IEnumerable<Intake> Intakes {get;set;} = [] ;
}