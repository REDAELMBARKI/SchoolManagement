using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Dtos.Responses;

public class SubjectResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Slug { get; set; } = string.Empty;
    

}
