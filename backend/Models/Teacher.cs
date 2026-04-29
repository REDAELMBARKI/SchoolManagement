using System.ComponentModel.DataAnnotations ;
using System.Text.Json.Serialization;
namespace SchoolManagement.Backend.Models ;   
  
  public class Teacher : Employee
    { 
        public int EmployeeId {get;set;}
        public string Specialization { get; set; } = null! ; 
       
        // navigation
        public ICollection<Group> Groups {get ;set ; } = new List<Group>() ;
    }