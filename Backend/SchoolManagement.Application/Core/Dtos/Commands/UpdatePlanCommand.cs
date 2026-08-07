using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Core.Dtos.Commands;

public class UpdatePlanCommand
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 120)]
    public int DurationMonths { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BaseAmount { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercent { get; set; }

    public bool IsActive { get; set; }

    [Range(0, 365)]
    public int RemainingAmountDueDays { get; set; }
}
