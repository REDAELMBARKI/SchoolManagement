using System.ComponentModel.DataAnnotations ;
using System.Text.Json.Serialization;
namespace SchoolManagement.Models ;   
  
  public class Teacher : Employee
    {
        
        
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
 
       
        [Required]
  
        public string Specialization { get; set; } = null! ; 
        public ICollection<Group> Groups {get ;set ; } = new List<Group>() ;
    }