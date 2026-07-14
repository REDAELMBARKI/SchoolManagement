using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Domain.Entities;

public abstract class Employee : BaseEntity
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Email? Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public int? GenderId { get; set; }
    public virtual Gender Gender { get; set; } = null!;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public decimal Salary { get; set; }
    // Foreign keys
    public int BranchId { get; set; }
    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
}
