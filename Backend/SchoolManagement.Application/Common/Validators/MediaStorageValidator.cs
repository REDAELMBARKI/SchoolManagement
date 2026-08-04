using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SchoolManagement.Application.Common.Settings;
using SchoolManagement.Domain.Common.Enums;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Common.Validators;

/// <summary>
/// Validates media files against storage governance rules.
/// Enforces file size limits, extension whitelist, MIME type whitelist, and branch quotas.
/// </summary>
public class MediaStorageValidator
{
    private readonly MediaStorageSettings _settings;
    private readonly IMediaRepository _mediaRepository;

    public MediaStorageValidator(
        IOptions<MediaStorageSettings> settings,
        IMediaRepository mediaRepository)
    {
        _settings = settings.Value;
        _mediaRepository = mediaRepository;
    }

    /// <summary>
    /// Validates a file against all storage governance rules for the specified MediaType.
    /// </summary>
    /// <param name="file">The uploaded file to validate</param>
    /// <param name="mediaType">The type of media (Photo, Video, Document, etc.)</param>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    
    public void ValidateFile(IFormFile file, MediaType mediaType)
    {
        if (file == null || file.Length == 0)
            throw new ValidationException("No file provided or file is empty.");

        // 1. Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLower();
        var allowedExtensions = _settings.GetAllowedExtensions(mediaType);
        
        if (!allowedExtensions.Contains(extension))
        {
            throw new ValidationException(
                $"File extension '{extension}' is not allowed for {mediaType}. " +
                $"Allowed extensions: {string.Join(", ", allowedExtensions)}");
        }

        // 2. Validate MIME type
        var mimeType = file.ContentType.ToLower();
        var allowedMimeTypes = _settings.GetAllowedMimeTypes(mediaType);
        
        if (!allowedMimeTypes.Contains(mimeType))
        {
            throw new ValidationException(
                $"MIME type '{mimeType}' is not allowed for {mediaType}. " +
                $"Allowed MIME types: {string.Join(", ", allowedMimeTypes)}");
        }

        // 3. Validate file size
        var maxFileSize = _settings.GetMaxFileSize(mediaType);
        
        if (file.Length > maxFileSize)
        {
            var maxSizeMB = maxFileSize / (1024.0 * 1024.0);
            var fileSizeMB = file.Length / (1024.0 * 1024.0);
            
            throw new ValidationException(
                $"File size ({fileSizeMB:F2} MB) exceeds maximum allowed size " +
                $"({maxSizeMB:F2} MB) for {mediaType}.");
        }
    }

    /// <summary>
    /// Validates that adding a new file won't exceed the branch's storage quota.
    /// </summary>
    /// <param name="branchId">The branch ID to check quota for</param>
    /// <param name="newFileSize">The size of the new file in bytes</param>
    /// <exception cref="ValidationException">Thrown when quota would be exceeded</exception>
    public async Task ValidateBranchQuotaAsync(Guid branchId, long newFileSize)
    {
        // Skip validation if quota is disabled (set to 0)
        if (_settings.BranchQuotaGB == 0)
            return;

        var quotaBytes = _settings.GetBranchQuotaBytes();
        var totalUsed = await _mediaRepository.GetTotalSizeByBranchAsync(branchId);
        var totalAfterUpload = totalUsed + newFileSize;

        if (totalAfterUpload > quotaBytes)
        {
            var usedGB = totalUsed / (1024.0 * 1024.0 * 1024.0);
            var quotaGB = quotaBytes / (1024.0 * 1024.0 * 1024.0);
            var newFileMB = newFileSize / (1024.0 * 1024.0);
            
            throw new ValidationException(
                $"Branch storage quota exceeded. " +
                $"Current usage: {usedGB:F2} GB, " +
                $"Quota: {quotaGB:F2} GB, " +
                $"New file size: {newFileMB:F2} MB. " +
                $"Please contact administrator to increase quota or delete unused media.");
        }
    }

    /// <summary>
    /// Gets a summary of branch storage usage.
    /// </summary>
    public async Task<BranchStorageSummary> GetBranchStorageSummaryAsync(Guid branchId)
    {
        var totalUsed = await _mediaRepository.GetTotalSizeByBranchAsync(branchId);
        var quotaBytes = _settings.GetBranchQuotaBytes();
        var percentUsed = quotaBytes > 0 ? (totalUsed / (double)quotaBytes) * 100 : 0;

        return new BranchStorageSummary
        {
            BranchId = branchId,
            TotalUsedBytes = totalUsed,
            TotalUsedMB = totalUsed / (1024.0 * 1024.0),
            TotalUsedGB = totalUsed / (1024.0 * 1024.0 * 1024.0),
            QuotaBytes = quotaBytes,
            QuotaGB = _settings.BranchQuotaGB,
            PercentUsed = percentUsed,
            RemainingBytes = quotaBytes - totalUsed,
            RemainingGB = (quotaBytes - totalUsed) / (1024.0 * 1024.0 * 1024.0)
        };
    }
}

/// <summary>
/// Summary of branch storage usage and quota.
/// </summary>
public class BranchStorageSummary
{
    public Guid BranchId { get; set; }
    public long TotalUsedBytes { get; set; }
    public double TotalUsedMB { get; set; }
    public double TotalUsedGB { get; set; }
    public long QuotaBytes { get; set; }
    public int QuotaGB { get; set; }
    public double PercentUsed { get; set; }
    public long RemainingBytes { get; set; }
    public double RemainingGB { get; set; }
}
