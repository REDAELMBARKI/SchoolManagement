using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Api.Controllers;

public class MediaController : ControllerBase
{
    private readonly IMediaService _main_service;

    public MediaController(IMediaService main_service)
    {
      this._main_service = main_service ;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(
        IFormFile file,
        MediaCollection collection ,
        MediaType mediaType
    )
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("No file provided or file is empty.");
        }

        // validation extension 
        string[] allowedExtentions = new [] {".png" , ".jpg" , ".jpeg"} ;
        string fileExt = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtentions.Contains(fileExt))
        {
            return BadRequest($"Invalid file extension. Allowed: {string.Join(", ", allowedExtentions)}");
        }

        // validate memetype 
        string[] allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        string fileMime = file.ContentType.ToLower();
        if (!allowedMimeTypes.Contains(fileMime))
        {
            return BadRequest($"Invalid MIME type. Allowed: {string.Join(", ", allowedMimeTypes)}");
        }

        MediaResponseDto media = await this._main_service.Upload(file, collection, mediaType);
        return Ok(media);
    }

}

 
