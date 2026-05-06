namespace SchoolManagement.Backend.Models;

public class Media : BaseEntity
{
    // file info
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;  // "image/jpeg", "video/mp4"
    public long Size { get; set; }  // in bytes
    public string? AltText { get; set; }  // for accessibility / SEO
    public int? Width { get; set; }  // pixels, for images
    public int? Height { get; set; } // pixels, for images

    // polymorphic owner
    public string OwnerType { get; set; } = string.Empty;  // "Student", "Teacher", "Branch"
    public int OwnerId { get; set; }

    // media classification
    public MediaType Type { get; set; }         // Photo, Banner, Document, Video
    public string? Collection { get; set; }     // "gallery", "profile", "certificates"
    public int Order { get; set; } = 0;         // sort order within collection
    public bool IsMain { get; set; } = false;   // is this the primary media?

    // storage info
    public string StorageProvider { get; set; } = string.Empty; // "Cloudinary", "S3", "Azure"
    public string PublicId { get; set; } = string.Empty;        // provider's own ID for deletion/manipulation
}

public enum MediaType
{
    Photo,
    Banner,
    Document,
    Video,
    Avatar
}