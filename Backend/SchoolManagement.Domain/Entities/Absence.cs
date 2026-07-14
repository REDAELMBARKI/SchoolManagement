
using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities;

 public class Absence : BaseEntity
    {
       
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
 
        // Absent / Late
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Absent";
 
        public bool IsJustified { get; set; } = false;
 
        [MaxLength(500)]
        public string? Reason { get; set; }

        
        // FKs
        public int StudentId { get; set; }
        public int ScheduleId { get; set; }
 
        // navigations
        public virtual Student Student { get; set; } = null!;
        public virtual Schedule Schedule { get; set; } = null!;
    }