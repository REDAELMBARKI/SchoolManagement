namespace SchoolManagement.Backend.Models;

public abstract class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? GenderId { get; set; }
    public Gender? Gender { get; set; }
}