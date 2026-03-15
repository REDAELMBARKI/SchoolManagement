

using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models ; 
 public abstract class User
    {
        public int Id { get; set; } 
 
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
 
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
 

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
 
        [Required,Phone ,  MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
 
        [Required, MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }
 

public  enum Role
{ Teacher  ,  Student , Parent  
}