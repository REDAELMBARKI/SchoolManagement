using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Http.Controllers;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Repositories;

namespace SchoolManagement.Backend.Services;

public class MediaService
{

    private readonly MediaRepository _main_repo;

    public MediaService(MediaRepository main_repo)
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

        return await _main_repo.Store(media);
    }
}
