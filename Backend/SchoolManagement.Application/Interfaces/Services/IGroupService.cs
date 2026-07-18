using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Domain.Interfaces.Services;

public interface IGroupService
{
    Task<GroupResponseDto> CreateAsync(GroupRequestDto dto);
    Task<GroupResponseDto?> GetByIdAsync(int id);
    Task<List<GroupResponseDto>> GetAllAsync();
    Task<GroupResponseDto?> UpdateAsync(int id, GroupRequestDto dto);
    Task<bool> DeleteAsync(int id);
}