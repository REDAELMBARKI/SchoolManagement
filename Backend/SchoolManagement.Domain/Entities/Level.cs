using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;

public class Level  : AggregateRoot
{
   

    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }

    public virtual Branch Branch {get;set;} = null! ;

    public int Order { get; set; } = 1;
}