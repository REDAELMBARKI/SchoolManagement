using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<List<PaymentResponseDto>> GetAllAsync();
    Task<PaymentResponseDto?> GetByIdAsync(Guid id);
    Task<PaymentResponseDto> CreateAsync(RegistrationPaymentCommand command);
    Task<PaymentResponseDto> SettleChargeAsync(ChargeSettlementPaymentCommand command);
    Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentCommand command);
    Task DeleteAsync(Guid id);
}
