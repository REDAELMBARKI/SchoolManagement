using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public abstract class Person : AggregateRoot
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? GenderId { get; set; }
    public virtual Gender? Gender { get; set; }
}