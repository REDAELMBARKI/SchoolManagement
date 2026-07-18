using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class MediaMapper
{
    public static MediaResponseDto ToResponse(Media media)
    {
        return new MediaResponseDto
        {
            Id = media.Id,
            Url = media.Url,
            FileName = Path.GetFileName(media.Url),
            MimeType = media.MimeType,
            Size = media.Size,
            AltText = media.AltText,
            Width = media.Width,
            Height = media.Height,
            MediaType = media.MediaType,
            Collection = media.Collection,
            IsMain = media.IsMain
        };
    }
}