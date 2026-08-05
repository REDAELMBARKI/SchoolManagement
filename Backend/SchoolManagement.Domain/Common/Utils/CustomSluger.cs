namespace SchoolManagement.Domain.Common.Utils;

public class CustomSluger
{
    public static async Task<string> Slug(IsRecordExists isRecordExistsDelegate, string slug)
    {
        int max_attempts = 5;
        var initSlug = slug;
        bool exists = await isRecordExistsDelegate(initSlug);
        while (exists && max_attempts-- > 0)
        {
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 6);
            initSlug = $"{slug}-{suffix}";
            exists = await isRecordExistsDelegate(initSlug);
        }
        if (exists)
        {
            string fullGuid = Guid.NewGuid().ToString("N");
            initSlug = $"{slug}-{fullGuid}";
        }
        return initSlug;
    }
}


public delegate Task<bool> IsRecordExists(string slug);
