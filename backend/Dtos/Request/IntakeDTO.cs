using System.ComponentModel.DataAnnotations;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Dtos;

public class IntakeDto 
{
    [Required, MinLength(3), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(3), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Gender")]
    public int GenderId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime IntakeDate { get; set; }

    [Required]
    public LeadSourceDto LeadSource { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid SchoolProgram")]
    public int SchoolProgramId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Branch")]
    public int BranchId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Agent")]
    public int? CommercialAgentId { get; set; }

    public IntakeStatus Status { get; set; } = IntakeStatus.New;

    public DateTime? FollowUpDate { get; set; }

    [MinLength(3), MaxLength(300)]
    public string? Notes { get; set; }
    
    [Required]
    public bool IsIndependent { get; set; } = false;
}


private class LeadSourceDto {

    [Required]
    public enum LeadSourceType { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid LeadSource")]
    public int LeadSourceId { get; set; }

}

public enum LeadSourceType {
    Opc,
    Ad,
}