
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IMediaService
{
    Task<MediaResponseDto> Upload(IFormFile file, MediaCollection collection, MediaType mediaType);
}