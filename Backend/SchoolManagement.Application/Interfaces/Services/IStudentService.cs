using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Domain.Interfaces.Services;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto> GetByIdAsync(int id);
    Task<StudentResponseDto> CreateAsync(StudentRequestDto dto);
    Task<StudentResponseDto> UpdateAsync(int id, StudentRequestDto dto);
    Task DeleteAsync(int id);
}