using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Entities;

public class Level  : BaseEntity
{
   

    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }

    public virtual Branch Branch {get;set;} = null! ;

    public int Order { get; set; } = 1;
}