using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ;   


  
 public class Session
    {
        public int Id { get; set; }
 
        [Required, MaxLength(50)]
        public string Label { get; set; } = string.Empty;
 
        [Required]
        public DateTime StartDate { get; set; }
 
        [Required]
        public DateTime EndDate { get; set; }
 
        public bool IsCurrent { get; set; } = false;
    }
 