using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponseDto>> GetAllAsync();
    Task<EnrollmentResponseDto?> GetByIdAsync(Guid id);
    Task<EnrollmentResponseDto> CreateAsync(EnrollmentCommand command);
    Task<EnrollmentResponseDto> UpdateAsync(Guid id, UpdateEnrollmentCommand command);
    Task<EnrollmentResponseDto> DropEnrollmentAsync(DropEnrollmentCommand command);
    Task<EnrollmentResponseDto> CompleteEnrollmentAsync(CompleteEnrollmentCommand command);
    Task<EnrollmentResponseDto> TransferGroupAsync(TransferGroupCommand command);
    Task<EnrollmentResponseDto> AddCreditAsync(Guid id, decimal amount);
    Task DeleteAsync(Guid id);
}
