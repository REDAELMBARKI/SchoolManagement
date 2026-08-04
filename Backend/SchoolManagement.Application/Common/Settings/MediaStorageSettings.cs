using SchoolManagement.Domain.Common.Enums;

namespace SchoolManagement.Application.Common.Settings;

/// <summary>
/// Configuration settings for media storage governance.
/// Controls file size limits, allowed extensions, MIME types per MediaType, and branch quotas.
/// </summary>
public class MediaStorageSettings
{
    /// <summary>
    /// Maximum file size in bytes for each MediaType.
    /// Example: Photo = 5242880 (5 MB)
    /// </summary>
    public Dictionary<string, long> MaxFileSizes { get; set; } = new();

    /// <summary>
    /// Allowed file extensions for each MediaType.
    /// Example: Photo = [".png", ".jpg", ".jpeg", ".gif", ".webp"]
    /// </summary>
    public Dictionary<string, string[]> AllowedExtensions { get; set; } = new();

    /// <summary>
    /// Allowed MIME types for each MediaType.
    /// Example: Photo = ["image/jpeg", "image/png", "image/gif", "image/webp"]
    /// </summary>
    public Dictionary<string, string[]> AllowedMimeTypes { get; set; } = new();

    /// <summary>
    /// Maximum total storage quota per branch in GB.
    /// Default: 10 GB. Set to 0 to disable quota enforcement.
    /// </summary>
    public int BranchQuotaGB { get; set; } = 10;

    /// <summary>
    /// Gets the max file size for a specific MediaType.
    /// </summary>
    public long GetMaxFileSize(MediaType mediaType)
    {
        var key = mediaType.ToString();
        return MaxFileSizes.TryGetValue(key, out var size) ? size : 5242880; // Default 5 MB
    }

    /// <summary>
    /// Gets the allowed extensions for a specific MediaType.
    /// </summary>
    public string[] GetAllowedExtensions(MediaType mediaType)
    {
        var key = mediaType.ToString();
        return AllowedExtensions.TryGetValue(key, out var extensions) 
            ? extensions 
            : new[] { ".png", ".jpg", ".jpeg" }; // Default
    }

    /// <summary>
    /// Gets the allowed MIME types for a specific MediaType.
    /// </summary>
    public string[] GetAllowedMimeTypes(MediaType mediaType)
    {
        var key = mediaType.ToString();
        return AllowedMimeTypes.TryGetValue(key, out var mimeTypes) 
            ? mimeTypes 
            : new[] { "image/jpeg", "image/png" }; // Default
    }

    /// <summary>
    /// Gets the branch quota in bytes.
    /// </summary>
    public long GetBranchQuotaBytes()
    {
        return (long)BranchQuotaGB * 1024 * 1024 * 1024;
    }
}
