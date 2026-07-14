

using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;
public class StudentParent : BaseEntity
{
    public int StudentId { get; set; }
    public int ParentId { get; set; }

    // navigations
    public virtual Student Student { get; set; } = null!;
    public virtual Parent Parent { get; set; } = null!;

}