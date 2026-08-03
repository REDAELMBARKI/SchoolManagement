<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;
=======
using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;
>>>>>>> 5fb5c4738af634e9e79c8340f0172f22f69d2a31

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class UpdateChargeRequestDto
{
    [Required]
    public Guid Id { get; set; }

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

    public Guid? AcademicTermId { get; set; }
}
