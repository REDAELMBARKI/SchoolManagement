using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Entities;
 public class Grade  : BaseEntity
    {
     
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
        public int BranchId { get; set; }

        // navigations
        public virtual Branch Branch {get;set;} = null! ;

        public virtual Student Student { get; set; } = null!;
        public virtual GroupTeacher GroupTeacher { get; set; } = null!;

    }
 