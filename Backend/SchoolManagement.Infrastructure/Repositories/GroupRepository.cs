using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
using SchoolManagement.Domain.Interfaces.Queries.Common;
namespace SchoolManagement.Infrastructure.Repositories;

public class GroupRepository : Repository<Group> , IGroupRepository
{
    public GroupRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<GroupResponseDto>> GetAllAsync()
    {
        var groups = await Query()
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .ToListAsync();

        return groups.Select(g => GroupMapper.MapGroup(g)).ToList();
    }

    public async Task<GroupResponseDto?> GetOneAsync(int id)
    {
        var group = await Query()
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group is null) return null;

        return GroupMapper.MapGroup(group);
    }

    public new async Task<GroupResponseDto> AddAsync(Group group)
    {
        await Context.AddAsync(group);
        await Context.SaveChangesAsync();
        return (await GetOneAsync(group.Id))!;
    }

    public async Task UpdateAsync(int id, Group group)
    {
        Group? dbGroup = await Context.Groups.FindAsync(id);
        if (dbGroup is null) throw new NotFoundException($"no group found with id {id}");
        group.Id = dbGroup.Id;
        Context.Entry(dbGroup).CurrentValues.SetValues(group);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Group? dbGroup = await Context.Groups.FindAsync(id);
        if (dbGroup is null) throw new NotFoundException($"no group found with id {id}");
        dbGroup.DeletedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
    }

    Task<Group> IRepository<Group>.AddAsync(Group entity)
    {
        throw new NotImplementedException();
    }

    Task<Group?> IRepository<Group>.UpdateAsync(int id, Group entity)
    {
        throw new NotImplementedException();
    }

    Task<List<Group>> IQuery<Group>.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Group?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
