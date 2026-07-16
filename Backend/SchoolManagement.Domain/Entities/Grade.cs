using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;
public class Grade  : AggregateRoot
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
        public Guid StudentId { get; set; }
        public Guid GroupTeacherId { get; set; }
        public Guid BranchId { get; set; }

        // navigations
        public virtual Branch Branch {get;set;} = null! ;
        public virtual Student Student { get; set; } = null!;

}
 