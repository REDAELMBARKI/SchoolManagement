using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class MediaFactory : Factory<Media>
{
    public MediaFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Media> Make()
    {
        var branches = await Context.Branches.Select(b => b.Id).ToListAsync();
        var students = await Context.Students.Select(s => s.Id).ToListAsync();
        var teachers = await Context.Teachers.Select(t => t.Id).ToListAsync();
        var users = await Context.Users.Select(u => u.Id).ToListAsync();

        var mediaTypes = Enum.GetValues<MediaType>();
        var collections = Enum.GetValues<MediaCollection>();
        var ownerTypes = new[] { "Student", "Teacher", "User", "Branch" };
        var storageProviders = new[] { "Local", "S3", "AzureBlob" };

        var ownerType = faker.PickRandom(ownerTypes);
        Guid? ownerId = null;
        switch (ownerType)
        {
            case "Student":
                ownerId = students.Any() ? faker.PickRandom(students) : Guid.Empty;
                break;
            case "Teacher":
                ownerId = teachers.Any() ? faker.PickRandom(teachers) : Guid.Empty;
                break;
            case "User":
                ownerId = users.Any() ? faker.PickRandom(users) : Guid.Empty;
                break;
            case "Branch":
                ownerId = branches.Any() ? faker.PickRandom(branches) : Guid.Empty;
                break;
        }

        var branchId = branches.Any() ? faker.PickRandom(branches) : Guid.Empty;
        var mediaType = faker.PickRandom(mediaTypes);
        var collection = mediaType switch
        {
            MediaType.Avatar => MediaCollection.Avatar,
            MediaType.Banner => MediaCollection.Banner,
            MediaType.Document => MediaCollection.Document,
            MediaType.Photo => MediaCollection.Photo,
            MediaType.Video => MediaCollection.Video,
            _ => MediaCollection.Unknown
        };

        var mimeType = mediaType switch
        {
            MediaType.Avatar or MediaType.Photo => faker.PickRandom("image/jpeg", "image/png"),
            MediaType.Banner => "image/png",
            MediaType.Document => faker.PickRandom("application/pdf", "application/msword"),
            MediaType.Video => "video/mp4",
            _ => "application/octet-stream"
        };

        var width = mediaType is MediaType.Avatar or MediaType.Photo or MediaType.Banner ? faker.Random.Int(200, 1920) : (int?)null;
        var height = mediaType is MediaType.Avatar or MediaType.Photo or MediaType.Banner ? faker.Random.Int(200, 1080) : (int?)null;

        return Media.Create(
            url: $"https://example.com/media/{Guid.NewGuid()}",
            mimeType: mimeType,
            size: faker.Random.Long(1024, 10485760), // 1KB to 10MB
            altText: faker.Lorem.Sentence(3),
            width: width,
            height: height,
            ownerType: ownerType,
            ownerId: ownerId.Value,
            mediaType: mediaType,
            collection: collection,
            order: faker.Random.Int(0, 10),
            isMain: faker.Random.Bool(),
            storageProvider: faker.PickRandom(storageProviders),
            branchId: branchId
        );
    }
}
