using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;

namespace SchoolManagement.Application.Services;

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
