namespace SchoolManagement.Domain.Interfaces.Queries.Common;

public interface ISluged
{
    Task<bool> IsExistsBySlugAsync(string slug);
}
