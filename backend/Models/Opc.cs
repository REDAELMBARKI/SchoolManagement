using SchoolManagement.Backend.Models;

public class Opc : Employee
{
    private int EmployeeId {get;set;}  
    private Employee Employee {get;set;}
    protected IEnumerable<Intake> Intakes {get;set;} = [] ;
}