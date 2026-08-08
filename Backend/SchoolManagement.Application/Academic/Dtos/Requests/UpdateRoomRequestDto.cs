using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Requests;

public class UpdateRoomRequestDto
{
    [Required, MinLength(1), MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Range(1, 1000)]
    public int Capacity { get; set; } = 20;
    
    [MaxLength(50)]
    public string? Floor { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
}
