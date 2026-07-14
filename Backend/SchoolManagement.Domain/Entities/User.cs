


using System.Text.Json.Serialization;


using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolManagement.Domain.Entities; 
 public class User  : Person
 {
       
        public string Email {get;set;} = string.Empty ;
        public DateOnly? DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty ;
        public bool IsActivated { get; set; }

 }

