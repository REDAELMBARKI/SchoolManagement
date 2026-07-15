using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Domain.Entities;

public abstract class Employee : Person
{

    public Email? Email { get; set; } 
    public string Phone { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public decimal Salary { get; set; }
    // Foreign keys
    public int BranchId { get; set; }
    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
}
