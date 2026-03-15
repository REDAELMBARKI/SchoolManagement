using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ;   
  
  public class Teacher : User
    {
        [Phone, MaxLength(20)]
        public string? Phone { get; set; }
 
        [Required]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        
        [Required]

        public string Specialization { get; set; } = null! ; 
        public ICollection<Group> Groups {get ;set ; } = new List<Group>() ;
    }