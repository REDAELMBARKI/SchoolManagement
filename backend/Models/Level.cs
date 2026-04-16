using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Models ;   

public class Level
{
    public int Id { get; set; }

    [Required, MaxLength(10)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(1, 20)]
    public int Order { get; set; } = 1;
}