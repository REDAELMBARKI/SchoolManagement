using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class AdLeadSourceRequestDto
{
    [Required]
    public Guid AdId { get; set; }
}
