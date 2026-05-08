using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

 public class Schedule : BaseEntity
{
        public int BranchId { get; set; }
        public int TeacherId { get; set; }
        public int RoomId { get; set; }
        public int DayId { get; set; }
        public int TimeSlotId { get; set; }
        public int GroupId {get; set; }
        public int SubjectId {get; set; }
 
        // navigations  
        public Branch Branch {get;set;} = null! ;
        public Subject Subject {get;set;} = null! ;
        public Group Group {get;set;} = null! ;
        public  Teacher Teacher { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public TimeSlot TimeSlot { get; set; } = null!;
        public Day Day { get; set; } = null!;
}


public class Day : BaseEntity
{
    public string Name { get; set; } = string.Empty;

}


public class TimeSlot : BaseEntity
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

}








