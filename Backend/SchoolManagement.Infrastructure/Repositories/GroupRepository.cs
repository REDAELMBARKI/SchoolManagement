using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Infrastructure.Repositories;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    public GroupRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Group> AddAsync(Group group)
    {
        return await base.AddAsync(group);
    }

    public async Task UpdateAsync(int id, Group group)
    {
        var dbGroup = await GetByIdForUpdateAsync(id);
        if (dbGroup is null) throw new NotFoundException($"No group found with id {id}");
        group.Id = dbGroup.Id;
        Context.Entry(dbGroup).CurrentValues.SetValues(group);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteAsync(id);
    }
}
