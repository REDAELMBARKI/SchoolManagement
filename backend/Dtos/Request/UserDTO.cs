using SchoolManagement.Models;

namespace SchoolManagement.DTOs;

public abstract class UserDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
    public int GenderId { get; set; }
}
