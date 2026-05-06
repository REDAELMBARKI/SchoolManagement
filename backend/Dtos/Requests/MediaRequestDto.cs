namespace SchoolManagement.Backend.Dtos.Requests;

public class MediaRequestDto
{
    // File data received from frontend
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    
    // Optional metadata
    public string? AltText { get; set; }
    public string MediaType { get; set; } = "Photo"; // Photo, Banner, Document, Video, Avatar
    public string? Collection { get; set; } // "gallery", "profile", "certificates"
    
    // Owner info - set by backend based on context or provided
    public string OwnerType { get; set; } = string.Empty;
    public int OwnerId { get; set; }
}
