using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities.EnrollmentAggregate;

public class Payment : BaseEntity
{
    public DateTime? PaidAt { get; set; } 
    public Guid EnrollmentId { get; set; }
    public decimal AmountPaid { get; set; }       
    public decimal FeeAmount { get; set; }        
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string Status { get; set; } = "Pending"; 

    public virtual Enrollment Enrollment { get; set; } = null!;
}