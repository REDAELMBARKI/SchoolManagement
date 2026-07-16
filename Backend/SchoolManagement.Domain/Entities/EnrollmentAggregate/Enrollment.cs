using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities.EnrollmentAggregate;
  
public class Enrollment  : AggregateRoot
    {
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";// Active / Dropped / Completed
        public string? Notes { get; set; } 
        // FKs
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid GroupId { get; set; }
        public Guid BranchId { get; set; }
        public Guid PlanId { get; set; }       // 1mo, 3mo, 6mo, 12mo
        // navigations
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual Subject Subject {get;set;} =null! ;
        public virtual Branch Branch {get;set;} = null! ;
        public virtual Plan Plan {get;set;} = null! ;
        public virtual Student Student { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
    }
