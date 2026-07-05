

using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 

 public class GroupTeacher : BaseEntity
    {
         // FKs
        public int TeacherId { get; set; }
        public int GroupId { get; set; }
        // navigations
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;

    }