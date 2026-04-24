
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 
public class Room
{
        public int Id { get; set; }
 
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
 
        [Required, Range(1, 100)]
        public int Capacity { get; set; } = 20;
 
        [MaxLength(10)]
        public string? Floor { get; set; }
 
        [MaxLength(300)]
        public string? Description { get; set; }
}