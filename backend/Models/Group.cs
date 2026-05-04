using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;   


  public class Group : BaseEntity
    {
 
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

        public ICollection<GroupTeacher> Teachers {get;set; } =  new List<GroupTeacher>() ; 
        public Branch Branch {get;set;} = null! ;

         // FKs
        public int BranchId { get; set; }

        public int LevelId { get; set; }
        public int LanguageId { get; set; }
        public int SessionId { get; set; }
 
        // navigations
        public Level Level { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
        public Session Session { get; set; } = null!;

  }
 