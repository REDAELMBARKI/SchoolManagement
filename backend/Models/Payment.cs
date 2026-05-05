   
   
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 
public class Payment : BaseEntity
{
    public int EnrollmentId { get; set; }
    public decimal AmountPaid { get; set; }        // what they actually paid now
    public decimal FeeAmount { get; set; }         // monthly fee for this period
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string Status { get; set; } = "Pending"; // Paid / Partial / Failed

    public Enrollment Enrollment { get; set; } = null!;
}