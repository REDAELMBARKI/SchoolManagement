
using Slugify;

namespace SchoolManagement.Backend.Utils;

public class CustomSluger
{
    public static string Slug(params string[] strs)
    {
        var helper = new SlugHelper();
        return helper.GenerateSlug(string.Join(" " ,strs));
    }
}