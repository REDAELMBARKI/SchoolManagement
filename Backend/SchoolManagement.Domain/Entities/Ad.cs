using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;

public class Ad : AggregateRoot
{
    public string Name { get; set; } = null! ;
    public string Slug {get;set;} = string.Empty ;
    public Guid PlatformId { get; set; }
    public Guid BranchId {get;set;}

    public virtual Branch Branch {get;set;} = null! ;
    public virtual Platform Platform { get; set; } = null! ;
}
