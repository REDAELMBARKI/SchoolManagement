namespace SchoolManagement.Application.Dtos.Responses;

public class GenderResponseDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
