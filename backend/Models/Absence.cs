
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ; 

 public class Absence
    {
        public int Id { get; set; }
 
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
        public Student Student { get; set; } = null!;
        public Schedule Schedule { get; set; } = null!;
    }