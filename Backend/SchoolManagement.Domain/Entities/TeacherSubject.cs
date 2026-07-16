using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;

public class TeacherSubject
{
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;

    public  Guid TeacherId  {get;set;}
    public  Guid SubjectId  {get;set;}

}
