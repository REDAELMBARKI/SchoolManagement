using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;  

public class TeacherSubject
{
    public Teacher Teacher { get; set; } = null!;
    public Subject Subject { get; set; } = null!;

    public  int TeacherId  {get;set;}
    public  int SubjectId  {get;set;}

}
