using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

public class Ad : BaseEntity
{
    public string Name { get; set; } = null! ;
    public string Slug {get;set;} = string.Empty ;
    
    public  Platform Platform { get; set; } = null! ;
    public int PlatformId { get; set; }
}
