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
        public virtual Branch Branch {get;set;} = null! ;
        public virtual Subject Subject {get;set;} = null! ;
        public virtual Group Group {get;set;} = null! ;
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
        public virtual TimeSlot TimeSlot { get; set; } = null!;
        public virtual Day Day { get; set; } = null!;
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








