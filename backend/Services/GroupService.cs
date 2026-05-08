using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Mappers;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Repositories;

namespace SchoolManagement.Backend.Services;

public class GroupService
{
    private readonly GroupRepository _repository;

    public GroupService(GroupRepository repository)
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
        return await _repository.GetOneAsync(id);
    }

    public async Task<List<GroupResponseDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<GroupResponseDto?> UpdateAsync(int id, GroupRequestDto dto)
    {
        var existing = await _repository.GetOneAsync(id);
        if (existing is null) throw new NotFoundException($"Group with id {id} not found");

        Group entity = GroupMapper.ToEntity(dto);
        entity.Id = id;
        await _repository.UpdateAsync(id, entity);
        return await _repository.GetOneAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        return true;
    }
}
