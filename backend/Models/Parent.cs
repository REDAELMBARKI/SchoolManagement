using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;   

 public class Parent  : Person
    {

        [Required, MaxLength(50)]
        public string Relationship { get; set; } = string.Empty;
        public ICollection<Student> Students  {get ; set ; } = new List<Student>();
    }
