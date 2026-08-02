<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;
=======
using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;
>>>>>>> 5fb5c4738af634e9e79c8340f0172f22f69d2a31

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class UpdatePaymentRequestDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public decimal? TransferFees { get; set; }

    public PaymentMethod Method { get; set; }

    public DateTime? PaidAt { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Guid BranchId { get; set; }
    public Guid ReceivedByStaffId { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}
