using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Media : AggregateRoot
{
    public string Url { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public long Size { get; private set; }
    public string? AltText { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public string OwnerType { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public MediaType MediaType { get; private set; }
    public MediaCollection? Collection { get; private set; }
    public int Order { get; private set; } = 0;
    public bool IsMain { get; private set; } = false;
    public string StorageProvider { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public virtual Branch Branch { get; private set; } = null!;

    private Media() { }

    public static Media Create(string url, string mimeType, long size, string? altText, int? width, int? height, string ownerType, Guid ownerId, MediaType mediaType, MediaCollection? collection, int order, bool isMain, string storageProvider, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException("URL cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new DomainException("Mime type cannot be empty.");
        }
        if (size < 0)
        {
            throw new DomainException("Size cannot be negative.");
        }
        if (string.IsNullOrWhiteSpace(ownerType))
        {
            throw new DomainException("Owner type cannot be empty.");
        }
        if (ownerId == Guid.Empty)
        {
            throw new DomainException("Owner ID must not be empty.");
        }
        if (string.IsNullOrWhiteSpace(storageProvider))
        {
            throw new DomainException("Storage provider cannot be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        return new Media
        {
            Url = url,
            MimeType = mimeType,
            Size = size,
            AltText = altText,
            Width = width,
            Height = height,
            OwnerType = ownerType,
            OwnerId = ownerId,
            MediaType = mediaType,
            Collection = collection,
            Order = order,
            IsMain = isMain,
            StorageProvider = storageProvider,
            BranchId = branchId
        };
    }

    public void UpdateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException("URL cannot be empty.");
        }
        Url = url;
    }

    public void UpdateMimeType(string mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new DomainException("Mime type cannot be empty.");
        }
        MimeType = mimeType;
    }

    public void UpdateSize(long size)
    {
        if (size < 0)
        {
            throw new DomainException("Size cannot be negative.");
        }
        Size = size;
    }

    public void UpdateAltText(string? altText)
    {
        AltText = altText;
    }

    public void UpdateWidth(int? width)
    {
        Width = width;
    }

    public void UpdateHeight(int? height)
    {
        Height = height;
    }

    public void UpdateOwnerType(string ownerType)
    {
        if (string.IsNullOrWhiteSpace(ownerType))
        {
            throw new DomainException("Owner type cannot be empty.");
        }
        OwnerType = ownerType;
    }

    public void UpdateOwnerId(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
        {
            throw new DomainException("Owner ID must not be empty.");
        }
        OwnerId = ownerId;
    }

    public void UpdateMediaType(MediaType mediaType)
    {
        MediaType = mediaType;
    }

    public void UpdateCollection(MediaCollection? collection)
    {
        Collection = collection;
    }

    public void UpdateOrder(int order)
    {
        Order = order;
    }

    public void UpdateIsMain(bool isMain)
    {
        IsMain = isMain;
    }

    public void UpdateStorageProvider(string storageProvider)
    {
        if (string.IsNullOrWhiteSpace(storageProvider))
        {
            throw new DomainException("Storage provider cannot be empty.");
        }
        StorageProvider = storageProvider;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }
}

public enum MediaType
{
    Photo,
    Banner,
    Document,
    Video,
    Avatar
}

public enum MediaCollection
{
    Unknown,
    Avatar,
    Banner,
    Document,
    Photo,
    Video,
}
