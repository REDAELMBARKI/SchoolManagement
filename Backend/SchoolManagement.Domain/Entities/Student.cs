using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolManagement.Domain.Entities;
    public class Student : Person
    {

        public string? Email {get;set;} = string.Empty;
        public string Phone {get;set;} = string.Empty;
        public DateOnly DateOfBirth { get; set; }

       // fks
        public Guid? IntakeId { get; set; }

        // navigation
        public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();
        public virtual ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();
        public virtual Intake? Intake { get; set; }

    }
 
