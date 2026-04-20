

namespace SchoolManagement.Models ;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SchoolManagement.Models;

class Admin : User
{
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string Specialization { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = new List<Group>();
}