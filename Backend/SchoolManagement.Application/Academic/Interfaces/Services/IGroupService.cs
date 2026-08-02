using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface IGroupService
{
    Task<GroupResponseDto> CreateAsync(GroupCommand command);
    Task<GroupResponseDto?> GetByIdAsync(Guid id);
    Task<List<GroupResponseDto>> GetAllAsync();
    Task<GroupResponseDto?> UpdateAsync(Guid id, UpdateGroupCommand command);
    Task<bool> DeleteAsync(Guid id);
}
