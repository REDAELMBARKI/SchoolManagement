  using System.ComponentModel.DataAnnotations ;
  namespace SchoolManagement.Backend.Models ; 
  
  public class SchoolConfig
    {
        public int Id { get; set; } 
 
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
 
        [MaxLength(255)]
        public string? Address { get; set; }
 
        [Phone, MaxLength(20)]
        public string? Phone { get; set; }
 
        [EmailAddress, MaxLength(255)]
        public string? Email { get; set; }
 
        [MaxLength(500)]
        public string? LogoUrl { get; set; }
 
        // "Primary" | "Language" | "Mixed"
        [Required, MaxLength(20)]
        public string SchoolType { get; set; } = string.Empty;
    }
 