

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Models ; 
 public class User  : Person
 {
        public string Email {get;set;} = string.Empty ;
        public string? Phone {get;set;} = string.Empty ;
        public DateOnly? DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty ;
        public bool IsActivated { get; set; }

 }

