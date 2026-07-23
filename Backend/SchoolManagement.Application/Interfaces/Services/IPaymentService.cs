using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<List<PaymentResponseDto>> GetAllAsync();
    Task<PaymentResponseDto?> GetByIdAsync(Guid id);
    Task<PaymentResponseDto> CreateAsync(PaymentRequestDto dto);
    Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentRequestDto dto);
    Task DeleteAsync(Guid id);
}