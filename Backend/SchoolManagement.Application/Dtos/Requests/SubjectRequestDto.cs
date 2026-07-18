namespace SchoolManagement.Application.Dtos.Requests;

public class SubjectRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid BranchId { get; set; }
}

