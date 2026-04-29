using System.ComponentModel.DataAnnotations;
using Scalar.AspNetCore;

namespace SchoolManagement.Backend.Models;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug {get;set;} = string.Empty ;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    
    public decimal Salary { get; set; }
    //fks
    public int? GenderId { get; set; }
     public int BranchId { get; set; }
    //nv
    public Gender? Gender { get; set; } 
    public Branch Branch {get;set;} = null! ;
    
    
}
