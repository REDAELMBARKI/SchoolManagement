namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface ISluged
{
    Task<bool> IsExistsBySlugAsync(string slug);
}
