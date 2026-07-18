using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Interfaces.Repositories;

public interface IMediaRepository 
{
    Task<Media> Add(Media media);
}
