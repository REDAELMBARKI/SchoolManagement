<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Core.Enums;
=======
using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;
>>>>>>> 5fb5c4738af634e9e79c8340f0172f22f69d2a31

namespace SchoolManagement.Application.Core.Dtos.Requests;

public class RegistrationPaymentRequestDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal AmountPaid { get; set; }
    public decimal? TransferFees { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ExternalReferenceCode { get; set; }
    public string? MethodDetailsJson { get; set; }
}
