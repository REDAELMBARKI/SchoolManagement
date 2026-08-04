using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
