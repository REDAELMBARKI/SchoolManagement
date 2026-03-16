using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Http.Requests;

public class CreateStudentRequest
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
        public string PasswordHash { get; set; } = string.Empty;
 
        [Required]
        public DateTime DateOfBirth { get; set; }
 
        [Required, MaxLength(10)]
        public string Gender { get; set; } = string.Empty;
 
}