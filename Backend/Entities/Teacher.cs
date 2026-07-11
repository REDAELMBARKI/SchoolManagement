using System.ComponentModel.DataAnnotations ;
using System.Text.Json.Serialization;
namespace SchoolManagement.Backend.Entities;
  
  public class Teacher : Employee
{

    public string Specialization { get; set; } = null!;
    
    // navigation
    public virtual ICollection<GroupTeacher> Groups {get ;set ; } = new List<GroupTeacher>() ;
    }