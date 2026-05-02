using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

public class Ad : BaseEntity
{
    public string Name { get; set; } = null! ;
    public string Slug {get;set;} = string.Empty ;
    public int PlatformId { get; set; }
    public int BranchId {get;set;}

    public Branch Branch {get;set;} = null! ;
    public  Platform Platform { get; set; } = null! ;
}
