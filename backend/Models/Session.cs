using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;   


  
 public class Session  :  BaseEntity
    {
 
        [Required, MaxLength(50)]
        public string Label { get; set; } = string.Empty;
 
        [Required]
        public DateTime StartDate { get; set; }
 
        [Required]
        public DateTime EndDate { get; set; }
 
        public bool IsCurrent { get; set; } = false;
    }
 