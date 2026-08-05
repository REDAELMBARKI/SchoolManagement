using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Enums;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _service;

    public MediaController(IMediaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Uploads a media file with owner validation and storage governance.
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="ownerId">The ID of the owner entity (Student, User, Teacher, etc.)</param>
    /// <param name="ownerType">The type of owner entity</param>
    /// <param name="collection">The media collection category</param>
    /// <param name="mediaType">The type of media (Photo, Video, Document, etc.)</param>
    /// <returns>MediaResponseDto with uploaded media details</returns>
    /// <response code="200">Media uploaded successfully</response>
    /// <response code="400">Validation error (invalid file type, size limit exceeded, quota exceeded)</response>
    /// <response code="404">Owner entity not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] Guid ownerId,
        [FromForm] OwnerType ownerType,
        [FromForm] MediaCollection collection,
        [FromForm] MediaType mediaType)
    {
        // Basic null/empty check
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = "No file provided or file is empty." });
        }

        try
        {
            var media = await _service.Upload(file, ownerId, ownerType, collection, mediaType);
            return Ok(media);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception (would need ILogger injected)
            return StatusCode(500, new { error = "An error occurred while uploading the file." });
        }
    }
}


