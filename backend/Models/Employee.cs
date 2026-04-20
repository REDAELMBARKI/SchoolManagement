using System.ComponentModel.DataAnnotations;
using Scalar.AspNetCore;

namespace SchoolManagement.Models ;

public class Employee : User
{

    [Required]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
        
    [Required]
    public  JobPosition Position  { get; set; }
}



public enum JobPosition
{
    Teacher,
    Administrator,
    Opc
}