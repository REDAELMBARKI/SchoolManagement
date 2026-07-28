using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IGroupService
{
    Task<GroupResponseDto> CreateAsync(GroupCommand command);
    Task<GroupResponseDto?> GetByIdAsync(Guid id);
    Task<List<GroupResponseDto>> GetAllAsync();
    Task<GroupResponseDto?> UpdateAsync(Guid id, UpdateGroupCommand command);
    Task<bool> DeleteAsync(Guid id);
}
