using System.ComponentModel.DataAnnotations ;
using System.Text.Json.Serialization;
namespace SchoolManagement.Backend.Models ;   
  
  public class Teacher : Employee
{
    public string Specialization { get; set; } = null!;
    
    // navigation
    public ICollection<GroupTeacher> Groups {get ;set ; } = new List<GroupTeacher>() ;
    }