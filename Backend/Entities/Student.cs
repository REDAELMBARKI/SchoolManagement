using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolManagement.Backend.Entities;
    public class Student : Person
    {

        public string? Email {get;set;} = string.Empty;
        public string Phone {get;set;} = string.Empty;
        public DateOnly DateOfBirth { get; set; }

       // fks
        public int? IntakeId { get; set; }

        // navigation
        public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
        public virtual ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();
        public virtual Intake? Intake { get; set; }

    }
 
