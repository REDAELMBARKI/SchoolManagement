using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data ;
namespace SchoolManagement.Infrastructure.Repositories;

public class MediaRepository : Repository<Media> , IMediaRepository
{
    public MediaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<MediaResponseDto> Add(Media media)
    {
        await Context.Medias.AddAsync(media);
        await Context.SaveChangesAsync();
        return new MediaResponseDto
        {
            Url = media.Url,
            MimeType = media.MimeType,
            Size = media.Size,
            Width = media.Width,
            Height = media.Height,
            MediaType = media.MediaType,
            Collection = media.Collection
        };
    }



}
