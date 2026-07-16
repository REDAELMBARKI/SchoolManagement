using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Entities;


namespace SchoolManagement.Application.Services;

public class GroupService
{
    private readonly IGroupRepository _repository;
    private readonly IGroupRepository _repository;

    public GroupService(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<GroupResponseDto> CreateAsync(GroupRequestDto dto)
    {
        Group entity = GroupMapper.ToEntity(dto);
        var newEntity = await _repository.AddAsync(entity);
        return newEntity;
    }

    public async Task<GroupResponseDto?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<GroupResponseDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<GroupResponseDto?> UpdateAsync(int id, GroupRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Group with id {id} not found");

        Group entity = GroupMapper.ToEntity(dto);
        entity.Id = id;
        await _repository.UpdateAsync(id, entity);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        return true;
    }
}
