

using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 
public class StudentParent : BaseEntity
{
    public int StudentId { get; set; }
    public int ParentId { get; set; }

    // navigations
    public Student Student { get; set; } = null!;
    public Parent Parent { get; set; } = null!;

}