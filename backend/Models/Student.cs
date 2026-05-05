using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Backend.Models ; 
    public class Student : Person
    {

        public string? Email {get;set;} = string.Empty;
        public string Phone {get;set;} = string.Empty;
        public DateOnly DateOfBirth { get; set; }

       // fks
        public int? IntakeId { get; set; }

        // navigation
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
        public ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();
        public Intake? Intake { get; set; }

    }
 
