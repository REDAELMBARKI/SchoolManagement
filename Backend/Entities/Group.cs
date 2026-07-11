using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Entities;

public class Group : BaseEntity
  {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } = 15;
        // Morning / Afternoon / Evening / Weekend
        public string Period { get; set; } = string.Empty;


         // FKs
        public int BranchId { get; set; }

        public int LevelId { get; set; }
        public int SubjectId { get; set; }
        // navigations
        public virtual ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();

        // many  to many (group -> teachers )
        public virtual ICollection<GroupTeacher> Teachers {get;set; } =  new List<GroupTeacher>() ;
        public virtual Branch Branch {get;set;} = null! ;
        public virtual Level Level { get; set; } = null!;
        public virtual Subject Subject { get; set; } = null!;

}