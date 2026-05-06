namespace SchoolManagement.Backend.Dtos.Responses;

public class MediaResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? AltText { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string? Collection { get; set; }
    public bool IsMain { get; set; }
}
