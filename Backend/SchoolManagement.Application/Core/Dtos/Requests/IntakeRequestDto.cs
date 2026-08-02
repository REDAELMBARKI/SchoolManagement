using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Domain.Common.ValueObjects;

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class IntakeRequestDto
{
    [Required, MinLength(3), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(3), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress, MaxLength(255)]
    public string? Email { get; set; } = null!;

    [Phone, MaxLength(20)]
    public string? Phone { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public Guid? GenderId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime IntakeDate { get; set; }

    [Required]
    public LeadSourceRequestDto LeadSource { get; set; } = null! ;

    [Required]
    public Guid SubjectId { get; set; }

    public Guid? ConvertedToStudentId { get; set; }

    public Guid? CommercialAgentId { get; set; }

    public IntakeStatus Status { get; set; } = IntakeStatus.New;

    public DateTime? FollowUpDate { get; set; }

    [MinLength(3), MaxLength(300)]
    public string? Notes { get; set; }
    
    [Required]
    public bool IsIndependent { get; set; } = false;

    public decimal TotalFees { get; set; }      
    public decimal AmountPaid { get; set; }        
    public decimal AmountRemaining => TotalFees - AmountPaid;
    public bool IsFullyPaid => AmountRemaining <= 0;
}
