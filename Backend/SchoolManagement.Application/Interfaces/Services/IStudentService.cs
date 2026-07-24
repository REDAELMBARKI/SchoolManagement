using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto> GetByIdAsync(Guid id);
    Task<StudentResponseDto> CreateAsync(StudentCommand command);
    Task<StudentResponseDto> UpdateAsync(Guid id, StudentRequestDto dto);
    Task DeleteAsync(Guid id);
}