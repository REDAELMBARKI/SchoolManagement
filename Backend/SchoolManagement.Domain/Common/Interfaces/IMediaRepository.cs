using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IMediaRepository 
{
    Task<Media> Add(Media media);
}
