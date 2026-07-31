using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IMediaRepository 
{
    Task<Media> Add(Media media);
}
