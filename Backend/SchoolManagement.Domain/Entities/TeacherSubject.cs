using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Entities;

public class TeacherSubject
{
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;

    public  int TeacherId  {get;set;}
    public  int SubjectId  {get;set;}

}
