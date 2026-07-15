
using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;
public class Room : AggregateRoot
{
 
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
 
        [Required, Range(1, 100)]
        public int Capacity { get; set; } = 20;
 
        [MaxLength(10)]
        public string? Floor { get; set; }
 
        [MaxLength(300)]
        public string? Description { get; set; }

        // fk

        public int BranchId { get; set; }
        public virtual Branch Branch {get;set;} = null! ;

}