
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
    Task<MediaResponseDto> Upload(IFormFile file, Guid OwnerId, OwnerType OwnerType, MediaCollection collection, MediaType mediaType);
}