<<<<<<< HEAD
﻿using SchoolManagement.Domain.Core.Enums;
=======
using SchoolManagement.Domain.Enums;
>>>>>>> 5fb5c4738af634e9e79c8340f0172f22f69d2a31

namespace SchoolManagement.Application.Core.Dtos.Responses;

public class EnrollmentResponseDto
{
    public Guid Id { get; set; }
    
    public DateTime EnrolledAt { get; set; }
    public DateTime? DroppedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active; // Active / Dropped / Completed
    
    public string? Notes { get; set; }

    public decimal CreditBalance { get; set; }
    
    // Foreign Keys 
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid GroupId { get; set; }
    public Guid BranchId { get; set; }
    
    // Navigation Properties
    public StudentResponseDto? Student { get; set; }
    public SubjectResponseDto? Subject { get; set; }
    public GroupResponseDto? Group { get; set; }
    public BranchResponseDto? Branch { get; set; }
    
    public ICollection<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
    public ICollection<EnrollmentPlanResponseDto> EnrollmentPlans { get; set; } = new List<EnrollmentPlanResponseDto>();
}
