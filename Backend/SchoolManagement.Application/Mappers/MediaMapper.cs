using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class MediaMapper
{
    public static Media ToDomain(MediaRequestDto dto, Guid ownerId, Guid branchId, int order = 0, bool isMain = false, string storageProvider = "Local")
    {
        var mediaType = Enum.TryParse<MediaType>(dto.MediaType, true, out var parsedMediaType)
            ? parsedMediaType
            : MediaType.Photo;

        var collection = dto.Collection != null && Enum.TryParse<MediaCollection>(dto.Collection, true, out var parsedCollection)
            ? parsedCollection
            : (MediaCollection?)null;

        return Media.Create(
            url: dto.Url,
            mimeType: dto.MimeType,
            size: dto.Size,
            altText: dto.AltText,
            width: dto.Width,
            height: dto.Height,
            ownerType: dto.OwnerType,
            ownerId: ownerId,
            mediaType: mediaType,
            collection: collection,
            order: order,
            isMain: isMain,
            storageProvider: storageProvider,
            branchId: branchId
        );
    }

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
