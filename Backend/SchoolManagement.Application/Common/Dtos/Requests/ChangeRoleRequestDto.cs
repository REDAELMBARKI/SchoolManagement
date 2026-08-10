using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class ChangeRoleRequestDto
{
    [Required]
    public string NewRole { get; set; } = string.Empty;  // SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent
}
