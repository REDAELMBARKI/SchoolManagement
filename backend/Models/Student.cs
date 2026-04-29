using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Backend.Models ; 
    public class Student : User
    {
       
        public DateOnly DateOfBirth { get; set; }
 
        public ICollection<Parent> Parents {get ; set; } = new List<Parent>(); 
        
        // fks
        public int LevelId {get ; set ;}
        public int GroupId {get ;  set ; } 
        
        // navigation
        public Group Group {get ;set ; } = null! ;
        public Level Level {get ;set ; } = null! ;

    }
 
