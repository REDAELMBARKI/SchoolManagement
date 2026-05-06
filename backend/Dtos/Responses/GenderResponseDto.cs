namespace SchoolManagement.Backend.Dtos.Responses;

public class GenderResponseDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
