using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class TransferGroupRequestDto
{
    [Required]
    public Guid NewGroupId { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;
}
