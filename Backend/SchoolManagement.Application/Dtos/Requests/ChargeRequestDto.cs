using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Requests;

public class ChargeRequestDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public ChargeType ChargeType { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateTime IssuedDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public string? Description { get; set; }

    public decimal AmountPaid { get; set; } = 0;

    public Guid? SourceId { get; set; }

    public Guid BranchId { get; set; }
}