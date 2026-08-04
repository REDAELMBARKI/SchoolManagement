using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Dtos.Requests;

/// <summary>
/// Request DTO for creating a student's parent/guardian (responsable).
/// </summary>
public class StudentResponsableRequestDto
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress, MaxLength(255)]
    public string? Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public RelationshipType Relationship { get; set; }

    public Guid? GenderId { get; set; }
}
