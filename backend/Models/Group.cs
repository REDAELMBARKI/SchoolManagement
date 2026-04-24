using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;   


  public class Group
    {
        public int Id { get; set; }
 
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
 
        [Required, Range(1, 60)]
        public int Capacity { get; set; } = 15;
 
        // Morning / Afternoon / Evening / Weekend
        [Required, MaxLength(20)]
        public string Period { get; set; } = string.Empty;

        // one  to many (group -> students )
        public ICollection<Student> Students {get;set; } =  new List<Student>() ; 
        // many  to many (group -> teachers )

        public ICollection<Teacher> Teachers {get;set; } =  new List<Teacher>() ; 

         // FKs
        public int LevelId { get; set; }
        public int LanguageId { get; set; }
        public int SessionId { get; set; }
 
        // navigations
        public Level Level { get; set; } = null!;
        public Language Language { get; set; } = null!;
        public Session Session { get; set; } = null!;

  }
 