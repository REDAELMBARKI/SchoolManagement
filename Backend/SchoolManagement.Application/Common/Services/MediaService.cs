using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;

namespace SchoolManagement.Application.Common.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _main_repo;

    public MediaService(IMediaRepository main_repo)
    {
        _main_repo = main_repo;
    }

    public async Task<MediaResponseDto> Upload(IFormFile file, MediaCollection collection, MediaType mediaType)
    {
        // Store the media
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
        string finalPath = Path.Combine(filePath, uniqueName);

        using (var stream = new FileStream(finalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Missing OwnerType, OwnerId, BranchId in parameters, but let's use placeholders for now
        // TODO: Update Upload method to accept OwnerType, OwnerId, BranchId
        Media media = Media.Create(
            url: $"/uploads/{uniqueName}",
            mimeType: file.ContentType,
            size: file.Length,
            altText: null,
            width: null, // TODO: Add ImageSharp package to get dimensions
            height: null, // TODO: Add ImageSharp package to get dimensions
            ownerType: "Unknown", // Temporary
            ownerId: Guid.Empty, // Temporary
            mediaType: mediaType,
            collection: collection,
            order: 0,
            isMain: false,
            storageProvider: "Local", // Temporary
            branchId: Guid.Empty // Temporary
        );

        Media storedMedia = await _main_repo.Add(media);

        return MediaMapper.ToResponse(storedMedia);
    }
}
