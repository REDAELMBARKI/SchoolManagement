using System.ComponentModel.DataAnnotations ;
using _.Models;
namespace SchoolManagement.Backend.Models ;   


  
 public class Session  :  BaseEntity
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
 