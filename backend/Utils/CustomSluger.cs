
using Slugify;

namespace SchoolManagement.Backend.Utils;

public class CustomSluger
{ 
  
    public static async Task<string> Slug(IsRecordExists isRecordExistsDelegate ,  params string[] strs  )
    {
         int max_attempts = 5; 
         var helper = new SlugHelper();
         string slug = helper.GenerateSlug(string.Join(" " ,strs));
         bool exists = await isRecordExistsDelegate(slug);
         while(exists && max_attempts-- > 0){
             string suffix = Guid.NewGuid().ToString("N").Substring(0, 6);
             slug = $"{slug}-{suffix}" ;
             exists = await isRecordExistsDelegate(slug);
        } 
        if (exists)
        {
            string fullGuid = Guid.NewGuid().ToString("N"); 
            slug = $"{slug}-{fullGuid}";
        }
        return slug;
    }
}


public delegate Task<bool> IsRecordExists(string slug) ; 