

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Models ; 
 public abstract class User : BaseEntity
    {
 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Slug {get;set;} = string.Empty ;
 
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } 

        public bool IsActivated { get; set; }         
        public string Phone { get; set; } = string.Empty;
        
        public int GenderId { get; set; }
        public Gender Gender { get; set; } = null! ;
    }
 

