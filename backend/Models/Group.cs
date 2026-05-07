using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;


  public class Group : BaseEntity
  {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } = 15;
        // Morning / Afternoon / Evening / Weekend
        public string Period { get; set; } = string.Empty;


         // FKs
        public int BranchId { get; set; }

        public int LevelId { get; set; }
        public int ScheduleId { get; set; }
        public int SubjectId { get; set; }
        // navigations
        public ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();

        // many  to many (group -> teachers )
        public ICollection<GroupTeacher> Teachers {get;set; } =  new List<GroupTeacher>() ;
        public Branch Branch {get;set;} = null! ;
        public Schedule Schedule { get; set; } = null!;
        public Level Level { get; set; } = null!;
        public Subject Subject { get; set; } = null!;

}