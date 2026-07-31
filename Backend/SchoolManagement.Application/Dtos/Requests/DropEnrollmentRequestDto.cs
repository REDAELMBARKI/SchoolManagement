using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Requests;

public class DropEnrollmentRequestDto
{
    [Required]
    public string Reason { get; set; } = string.Empty;
}
