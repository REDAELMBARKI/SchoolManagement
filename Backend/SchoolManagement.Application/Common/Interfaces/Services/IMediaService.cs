
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Enums;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IMediaService
{
    /// <summary>
    /// Uploads a media file with owner validation and storage governance.
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="ownerId">The ID of the owner entity (Student, User, Teacher, etc.)</param>
    /// <param name="ownerType">The type of owner entity</param>
    /// <param name="collection">The media collection category</param>
    /// <param name="mediaType">The type of media (Photo, Video, Document, etc.)</param>
    /// <returns>MediaResponseDto containing the uploaded media details</returns>
    /// <exception cref="ValidationException">Thrown when file validation fails or quota exceeded</exception>
    /// <exception cref="NotFoundException">Thrown when owner entity doesn't exist</exception>
    Task<MediaResponseDto> Upload(IFormFile file, Guid ownerId, OwnerType ownerType, MediaCollection collection, MediaType mediaType);
}