using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Backend.Models ; 
    public class Student : Person
    {

        public string? Email {get;set;} = string.Empty;
        public string Phone {get;set;} = string.Empty;
        public DateOnly DateOfBirth { get; set; }

        // Navigation to StudentParent join entities
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
        
        // fks
        public int LevelId {get ; set ;}
        public int GroupId {get ;  set ; }
        public int? IntakeId { get; set; }

        // navigation
        public Group Group {get ;set ; } = null! ;
        public Level Level {get ;set ; } = null! ;
        public Intake? Intake { get; set; }

    }
 
