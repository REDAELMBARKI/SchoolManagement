using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Academic.Dtos.Commands;

public class RoomCommand
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 500)]
    public int Capacity { get; set; } = 20;

    [MaxLength(50)]
    public string? Floor { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
