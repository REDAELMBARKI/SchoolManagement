using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;
  
public class Enrollment  : BaseEntity
    {
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";// Active / Dropped / Completed
        public string? Notes { get; set; } 
        // FKs
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public int BranchId { get; set; }
        public int PlanId { get; set; }       // 1mo, 3mo, 6mo, 12mo
        // navigations
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual Subject Subject {get;set;} =null! ;
        public virtual Branch Branch {get;set;} = null! ;
        public virtual Plan Plan {get;set;} = null! ;
        public virtual Student Student { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
    }
