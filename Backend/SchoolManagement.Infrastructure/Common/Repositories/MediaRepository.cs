using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class MediaRepository : Repository<Media>, IMediaRepository
{
    public MediaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Media> Add(Media media)
    {
        await Context.Medias.AddAsync(media);
        await Context.SaveChangesAsync();
        return (media);
    }

    public async Task<long> GetTotalSizeByBranchAsync(Guid branchId)
    {
        var totalSize = await Context.Medias
            .Where(m => m.BranchId == branchId)
            .SumAsync(m => m.Size);

        return totalSize;
    }
}
