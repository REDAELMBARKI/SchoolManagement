using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

 public class Schedule : BaseEntity
{
        public int BranchId { get; set; }
        public int TeacherId { get; set; }
        public int RoomId { get; set; }
        public int DayId { get; set; }
        public int TimeSlotId { get; set; }
 
        // navigations
        public Branch Branch {get;set;} = null! ;
        public  Teacher Teacher { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public TimeSlot TimeSlot { get; set; } = null!;
        public Day Day { get; set; } = null!;
}


public class Day : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // navigations\

    public ICollection<TimeSlot> TimeSlots  = new List<TimeSlot>();
}


public class TimeSlot : BaseEntity
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public Schedule schedule



  /*
    Time Plam For A student 
    
    groupSchedule {
     
        day=monday : {
            timeslot : 
            {
              Room
              Teacher
              
               
            },
            timeslot : 
            {
            
            } 
                    
                    
        }
                
     }
    */





}








