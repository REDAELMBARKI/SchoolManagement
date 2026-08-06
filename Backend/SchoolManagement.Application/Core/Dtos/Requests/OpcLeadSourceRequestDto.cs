using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class OpcLeadSourceRequestDto
{
    [Required]
    public Guid OpcId { get; set; }
}
