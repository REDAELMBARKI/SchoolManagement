using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

 public class Schedule
{
        public int Id { get; set; }
 
        // Monday / Tuesday / ...
        [Required, MaxLength(15)]
        public string DayOfWeek { get; set; } = string.Empty;
 
        [Required]
        public TimeSpan StartTime { get; set; }
 
        [Required]
        public TimeSpan EndTime { get; set; }
 
        // hex color for the planner UI e.g. "#4CAF50"
        [MaxLength(10)]
        public string? Color { get; set; }

         // FKs
        public int GroupTeacherId { get; set; }
        public int RoomId { get; set; }
 
        // navigations
        public GroupTeacher GroupTeacher { get; set; } = null!;
        public Room Room { get; set; } = null!;
}
 