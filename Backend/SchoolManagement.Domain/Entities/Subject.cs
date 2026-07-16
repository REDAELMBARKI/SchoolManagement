using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities;

public class Subject : AggregateRoot
{
        public string Name { get; set; } = string.Empty;
        public string Slug {get;set;} = string.Empty ;

        public string? Description { get; set; }

        public Guid BranchId { get; set; }

        // navigation
        public virtual ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();
     
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        public virtual Branch Branch {get;set;} = null! ;
}