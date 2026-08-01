using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class RefundCommand
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Refund amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required, MinLength(3), MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    // Populated by the service from the current user context
    public Guid RefundedByStaffId { get; set; }
    public Guid BranchId { get; set; }
}
