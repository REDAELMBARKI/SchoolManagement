using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Models ; 
    public class Student : User
    {
        [Required]
        public DateTime DateOfBirth { get; set; }
 
        [Required, MaxLength(10)]
        public string Gender { get; set; } = string.Empty;
 
        [Required, MaxLength(50)]
        public string NationalId { get; set; } = string.Empty;
 
        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        [Required]
        public ICollection<Parent> Parents {get ; set; } = new List<Parent>(); 

        public Group Group {get ;set ; } = null! ;
        public int GroupId {get ;  set ; } 

    }
 
