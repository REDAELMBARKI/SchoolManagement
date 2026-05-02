

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Models ; 
 public class User  : Person
 {
        public string? Password { get; set; }
        public bool IsActivated { get; set; }

 }

