using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Services;

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

        using (var stream = new FileStream(finalPath , FileMode.Create))
        {
             file.CopyToAsync(stream);
        }


        Media media = new Media
        {
            Url = $"/uploads/{uniqueName}",
            MimeType = file.ContentType,
            Size = file.Length,
            Width = null, // TODO: Add ImageSharp package to get dimensions
            Height = null, // TODO: Add ImageSharp package to get dimensions
            MediaType = mediaType,
            Collection = collection
            
        };

        return await _main_repo.Add(media);
    }
}
