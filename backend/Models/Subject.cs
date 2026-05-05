
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models;

public class Subject : BaseEntity
{
        public string Name { get; set; } = string.Empty;
        public string Slug {get;set;} = string.Empty ;

        public string? Description { get; set; }

        public int BranchId { get; set; }

        // navigation
        public ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();

        public Branch Branch {get;set;} = null! ;
}