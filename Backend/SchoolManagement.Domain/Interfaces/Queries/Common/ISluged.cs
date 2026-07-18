namespace SchoolManagement.Domain.Interfaces.Queries.Common;

public interface ISluged
{
    Task<bool> IsExistsBySlug(string slug);
}
