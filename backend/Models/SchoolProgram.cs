
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models;

 public class SchoolProgram
    {
        public int Id { get; set; }
 
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
 
        public string Slug {get;set;} = string.Empty ;

        [MaxLength(300)]
        public string? Description { get; set; }
    }