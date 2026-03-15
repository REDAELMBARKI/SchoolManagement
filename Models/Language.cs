using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ; 
public class Language
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

}