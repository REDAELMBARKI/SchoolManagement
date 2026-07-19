using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class MediaRepository : Repository<Media>, IMediaRepository
{
    public MediaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Media> Add(Media media)
    {
        await Context.Medias.AddAsync(media);
        await Context.SaveChangesAsync();
        return (media);
    }
}
