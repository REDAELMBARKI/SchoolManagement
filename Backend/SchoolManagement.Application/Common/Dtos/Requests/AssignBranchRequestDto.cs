using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Common.Dtos.Requests;

public class AssignBranchRequestDto
{
    [Required]
    public Guid BranchId { get; set; }
}
