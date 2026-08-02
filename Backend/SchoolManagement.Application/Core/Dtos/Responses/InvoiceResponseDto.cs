<<<<<<< HEAD
﻿using SchoolManagement.Domain.Core.Enums;
=======
using SchoolManagement.Domain.Enums;
>>>>>>> 5fb5c4738af634e9e79c8340f0172f22f69d2a31

namespace SchoolManagement.Application.Core.Dtos.Responses;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal CreditAppliedAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public Guid BranchId { get; set; }
    public ChargeResponseDto? Charge { get; set; }
}
