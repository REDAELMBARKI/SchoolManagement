using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Responses;

public class GroupTeacherResponseDto
{
    public Guid Id { get; set; }
    
    // Foreign Keys
    public Guid TeacherId { get; set; }
    public Guid GroupId { get; set; }
    
    // Navigation Properties
    public TeacherResponseDto? Teacher { get; set; }
    public GroupResponseDto? Group { get; set; }
}
