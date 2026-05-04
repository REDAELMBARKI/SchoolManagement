
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models;

 public class Subject : BaseEntity
    {
    
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
 
        public string Slug {get;set;} = string.Empty ;

        [MaxLength(300)]
        public string? Description { get; set; }
   
        public int BranchId { get; set; }
        public Branch Branch {get;set;} = null! ;
    }