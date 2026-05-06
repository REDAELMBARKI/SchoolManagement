using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;    
  
public class Enrollment  : BaseEntity
    {
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";// Active / Dropped / Completed
        public string? Notes { get; set; } 
        // FKs
        public int StudentId { get; set; }
        public int SubjectId {get;set;}
        public int GroupId { get; set; }
        public int BranchId { get; set; }
        public int PlanId { get; set; }       // 1mo, 3mo, 6mo, 12mo
        // navigations
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public Subject Subject {get;set;} =null! ;
        public Branch Branch {get;set;} = null! ;
        public Plan Plan {get;set;} = null! ;
        public Student Student { get; set; } = null!;
        public Group Group { get; set; } = null!;
    }
