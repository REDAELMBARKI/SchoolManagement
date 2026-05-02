using System.ComponentModel.DataAnnotations;
using Scalar.AspNetCore;

namespace SchoolManagement.Backend.Models;

public class Employee : Person
{
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public decimal Salary { get; set; }
    //fks
     public int BranchId { get; set; }
    //nv
    public Branch Branch {get;set;} = null! ;

}
