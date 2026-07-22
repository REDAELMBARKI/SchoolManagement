using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Application.Services.Groups;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IGroupQueryService _query;

    public GroupService(IGroupRepository repository, IGroupQueryService query)
    {
        _repository = repository;
        _query = query;
    }

    public async Task<GroupResponseDto> CreateAsync(GroupRequestDto dto)
    {
        Group entity = GroupMapper.ToDomain(dto);
        var newEntity = await _repository.AddAsync(entity);
        return GroupMapper.ToResponse(newEntity);
    }

    public async Task<GroupResponseDto?> GetByIdAsync(Guid id)
    {
        return await _query.GetResponseByIdAsync(id);
    }

    public async Task<List<GroupResponseDto>> GetAllAsync()
    {
        return await _query.GetAllResponsesAsync();
    }
    
    public async Task<GroupResponseDto?> UpdateAsync(Guid id, GroupRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Group with id {id} not found");

        var entity = GroupMapper.ToDomain(dto);
        entity.Id = id;
        var updated = await _repository.UpdateAsync(entity);
        return GroupMapper.ToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        return true;
    }
}
