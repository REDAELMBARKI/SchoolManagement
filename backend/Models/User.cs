

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using _.Models;

namespace SchoolManagement.Models ; 
 public abstract class User
    {
        public int Id { get; set; } 
 
        public string FirstName { get; set; } = string.Empty;
 
        public string LastName { get; set; } = string.Empty;
 
 
        public string Email { get; set; } = string.Empty;
 
        public string Phone { get; set; } = string.Empty;
        
        public int GenderId { get; set; }
        public Gender Gender { get; set; } = null! ;
    }
 

