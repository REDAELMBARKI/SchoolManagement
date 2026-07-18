using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Interfaces.Repositories;

public interface IMediaRepository 
{
    Task<MediaResponseDto> Add(Media media);
}
