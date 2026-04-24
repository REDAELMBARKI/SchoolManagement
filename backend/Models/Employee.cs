using System.ComponentModel.DataAnnotations;
using Scalar.AspNetCore;

namespace SchoolManagement.Backend.Models;

public class Employee : User
{

    [Required]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public decimal Salary { get; set; }
    
}



public enum JobPosition
{
    Teacher,
    Administrator,
    Opc
}