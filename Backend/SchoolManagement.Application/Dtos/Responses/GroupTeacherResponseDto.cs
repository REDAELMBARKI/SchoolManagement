using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Backend.Dtos.Responses;

public class GroupTeacherResponseDto
{
    public int Id { get; set; }
    
    // Foreign Keys
    public int TeacherId { get; set; }
    public int GroupId { get; set; }
    
    // Navigation Properties
    public TeacherResponseDto? Teacher { get; set; }
    public GroupResponseDto? Group { get; set; }
}
