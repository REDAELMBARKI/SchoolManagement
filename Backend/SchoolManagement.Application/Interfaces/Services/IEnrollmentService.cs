using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponseDto>> GetAllAsync();
    Task<EnrollmentResponseDto?> GetByIdAsync(Guid id);
    Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto dto);
    Task<EnrollmentResponseDto> UpdateAsync(Guid id, EnrollmentRequestDto dto);
    Task DeleteAsync(Guid id);
}