   
   
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ; 
public class Payment  : BaseEntity
    {
 
        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; } = 0m;
 
        [Required]
        public DateTime DueDate { get; set; }
 
        public DateTime? PaymentDate { get; set; }
 
        // Cash / Transfer / Cheque
        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = "Cash";
 
        // Paid / Pending / Partial / Cancelled
        [Required, MaxLength(30)]
        public string Status { get; set; } = "Pending";
 
        [MaxLength(300)]
        public string? Note { get; set; }

         // FKs
        public int StudentId { get; set; }
        public int SessionId { get; set; }
        public int BranchId { get; set; }

        // navigations
        public Branch Branch {get;set;} = null! ;

        public Student Student { get; set; } = null!;
        public Session Session { get; set; } = null!;
}
 