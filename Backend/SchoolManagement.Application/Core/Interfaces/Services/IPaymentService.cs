using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IPaymentService
{
    Task<List<PaymentResponseDto>> GetAllAsync();
    Task<PaymentResponseDto?> GetByIdAsync(Guid id);
    Task<PaymentResponseDto> CreateAsync(RegistrationPaymentCommand command);
    Task<PaymentResponseDto> SettleChargeAsync(ChargeSettlementPaymentCommand command);
    Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentCommand command);
    Task DeleteAsync(Guid id);
}
