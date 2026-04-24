using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

public class Ad
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public  Platform Platform { get; set; }
    public int PlatformID { get; set; }
}
