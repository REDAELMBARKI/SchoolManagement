
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IMediaService
{
    Task<MediaResponseDto> Upload(IFormFile file, MediaCollection collection, MediaType mediaType);
}