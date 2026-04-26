using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

public class Ad
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null! ;
    public string Slug {get;set;} = string.Empty ;
    
    public  Platform Platform { get; set; } = null! ;
    public int PlatformId { get; set; }
}
