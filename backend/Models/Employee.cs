using System.ComponentModel.DataAnnotations;
using Scalar.AspNetCore;

namespace SchoolManagement.Backend.Models;

public class Employee : BaseEntity
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public int? GenderId { get; set; }
    public Gender Gender { get; set; } = null!;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public decimal Salary { get; set; }
    // Foreign keys
    public int BranchId { get; set; }
    // Navigation properties
    public Branch Branch { get; set; } = null!;
}
