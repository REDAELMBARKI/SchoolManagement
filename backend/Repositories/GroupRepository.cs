using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Mappers;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class GroupRepository : Repository<Group>
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
                .ThenInclude(gt => gt.Teacher)
            .ToListAsync();

        return groups.Select(g => GroupMapper.MapGroup(g)).ToList();
    }

    public async Task<GroupResponseDto?> GetOneAsync(int id)
    {
        var group = await Query()
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
                .ThenInclude(gt => gt.Teacher)
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


}
