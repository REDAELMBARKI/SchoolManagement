using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;   
 public class Grade
    {
        public int Id { get; set; }
 
        // Quiz / Exam / Oral / Project
        [Required, MaxLength(50)]
        public string EvaluationType { get; set; } = string.Empty;
 
        [Required, Range(0, 100)]
        public float Score { get; set; } = 0f;
 
        public float MaxScore { get; set; } = 100f;
 
        [Required]
        public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
 
        [MaxLength(300)]
        public string? Comment { get; set; }

                // FKs
        public int StudentId { get; set; }
        public int GroupTeacherId { get; set; }
 
        // navigations
        public Student Student { get; set; } = null!;
        public GroupTeacher GroupTeacher { get; set; } = null!;

    }
 