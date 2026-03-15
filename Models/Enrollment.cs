using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ;    
  
public class Enrollment
    {
        public int Id { get; set; }
 
        [Required]
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
 
        // Active / Dropped / Completed
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Active";

            // FKs
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public int SessionId { get; set; }
 
        // navigations
        public Student Student { get; set; } = null!;
        public Group Group { get; set; } = null!;
        public Session Session { get; set; } = null!;
    }
