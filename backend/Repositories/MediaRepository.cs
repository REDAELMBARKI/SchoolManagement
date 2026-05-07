using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class MediaRepository : Repository<Media>
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
