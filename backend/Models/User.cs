

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolManagement.Models ; 
 public abstract class User
    {
        public int Id { get; set; } 
 
        public string FirstName { get; set; } = string.Empty;
 
        public string LastName { get; set; } = string.Empty;
 
        public string Gender { get; set; } = string.Empty;
 
        public string Email { get; set; } = string.Empty;
 
        public string Phone { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
 
        public Role Role { get; set; } 
    }
 

public  enum Role
{ Teacher  ,  Student , Parent  
}