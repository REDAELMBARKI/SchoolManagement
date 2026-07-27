using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Dtos.Commands;

public class StudentResponsableCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? GenderId { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public RelationshipType Relationship { get; set; }
    public Guid BranchId { get; set; }
}
