

using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ; 
public class StudentParent
{
    public int Id { get; set; }    // FKs
    public int StudentId { get; set; }
    public int ParentId { get; set; }

    // navigations
    public Student Student { get; set; } = null!;
    public Parent Parent { get; set; } = null!;
    

}
 