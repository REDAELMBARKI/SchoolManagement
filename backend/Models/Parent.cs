using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Backend.Models;

public class Parent : Person
{
    public string? Email {get;set;} = string.Empty;
    public string Phone {get;set;} = string.Empty;
    public RelationshipType Relationship { get; set; }
    public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
}



public enum RelationshipType
{
    Father,
    Mother,
    Guardian,
    Grandfather,
    Grandmother,
    Uncle,
    Aunt,
    Other
}
