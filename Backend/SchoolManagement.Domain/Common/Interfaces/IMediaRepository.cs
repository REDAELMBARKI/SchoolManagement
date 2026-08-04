using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Common.Interfaces;

public interface IMediaRepository 
{
    Task<Media> Add(Media media);

    /// <summary>
    /// Gets the total storage size in bytes used by all media files for a specific branch.
    /// Used for enforcing branch storage quotas.
    /// </summary>
    /// <param name="branchId">The branch ID to calculate total storage for</param>
    /// <returns>Total size in bytes</returns>
    Task<long> GetTotalSizeByBranchAsync(Guid branchId);
}
