using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponseDto>> GetAllAsync();
    Task<EnrollmentResponseDto?> GetByIdAsync(Guid id);
    Task<EnrollmentResponseDto> CreateAsync(EnrollmentCommand command);
    Task<EnrollmentResponseDto> UpdateAsync(Guid id, UpdateEnrollmentCommand command);
    Task<EnrollmentResponseDto> AddCreditAsync(Guid id, decimal amount);
    Task DeleteAsync(Guid id);
}
