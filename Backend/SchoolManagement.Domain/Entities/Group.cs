using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities;

public class Group : AggregateRoot
  {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } = 15;
        // Morning / Afternoon / Evening / Weekend
        public string Period { get; set; } = string.Empty;


         // FKs
        public Guid BranchId { get; set; }

        public Guid LevelId { get; set; }
        public Guid SubjectId { get; set; }
        // navigations
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<GroupTeacher> Teachers { get; set; } = new List<GroupTeacher>();
        public virtual Branch Branch { get; set; } = null!;
        public virtual Level Level { get; set; } = null!;
        public virtual Subject Subject { get; set; } = null!;
}