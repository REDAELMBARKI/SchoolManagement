using System.ComponentModel.DataAnnotations ;
using System.Text.Json.Serialization;
namespace SchoolManagement.Domain.Entities;
  
 public class Teacher : Employee
{

        public string Specialization { get; set; } = null!;
    
        // navigation

        public virtual ICollection<Group> Groups {get ;set ; } = new List<Group>() ;
    }