namespace SchoolManagement.Backend.Dtos.Requests;

public class MediaRequestDto
{
    // File data received from frontend
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? AltText { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string MediaType { get; set; } = "Photo"; // Photo, Banner, Document, Video, Avatar
    public string? Collection { get; set; } // "gallery", "principal", "certificates"
    // Owner info - set by backend based on context or provided
    public string OwnerType { get; set; } = string.Empty;
    public int OwnerId { get; set; }
}
