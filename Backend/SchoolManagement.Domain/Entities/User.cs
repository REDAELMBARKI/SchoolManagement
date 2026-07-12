


using System.Text.Json.Serialization;
using SchoolManagement.Backend.Entities;

using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolManagement.Backend.Entities; 
 public class User  : Person
 {
       
        public string Email {get;set;} = string.Empty ;
        public DateOnly? DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty ;
        public bool IsActivated { get; set; }

 }

