using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IGroupService
{
    Task<GroupResponseDto> CreateAsync(GroupRequestDto dto);
    Task<GroupResponseDto?> GetByIdAsync(Guid id);
    Task<List<GroupResponseDto>> GetAllAsync();
    Task<GroupResponseDto?> UpdateAsync(Guid id, GroupRequestDto dto);
    Task<bool> DeleteAsync(Guid id);
}