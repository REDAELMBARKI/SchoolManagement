namespace SchoolManagement.Backend.Entities;

public abstract class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? GenderId { get; set; }
    public virtual Gender? Gender { get; set; }
}