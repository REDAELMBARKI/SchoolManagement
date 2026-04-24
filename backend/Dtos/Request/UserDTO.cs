using System.ComponentModel.DataAnnotations;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Dtos;

public  class UserDto
{
 
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
 
        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
 
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
 
        [Required,Phone ,  MaxLength(255)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }
 
        [Required]
        public int GenderId { get; set; }

        public bool IsActivated { get; set; } = false;


}
